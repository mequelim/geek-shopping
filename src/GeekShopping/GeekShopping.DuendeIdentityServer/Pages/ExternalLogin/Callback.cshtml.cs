using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeekShopping.DuendeIdentityServer.Pages.ExternalLogin
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class Callback : PageModel
    {
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<Callback> _logger;
        private readonly IEventService _events;

        public Callback(
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<Callback> logger,
            TestUserStore? users = null
        )
        {
            // This is where you would plug in your own custom identity management library (e.g., ASP.NET Identity):
            _users = users
                     ?? throw new InvalidOperationException(
                         "Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController."
                     );

            _interaction = interaction;
            _logger = logger;
            _events = events;
        }

        public async Task<IActionResult> OnGet()
        {
            // Read external identity from the temporary cookie:
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if(result.Succeeded is not true) throw new InvalidOperationException($"External authentication error: {result.Failure}");

            ClaimsPrincipal externalUser = result.Principal ??
                                           throw new InvalidOperationException("External authentication produced a null Principal");

            if(_logger.IsEnabled(LogLevel.Debug))
            {
                IEnumerable<string> externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.ExternalClaims(externalClaims);
            }

            // Look up our user and external provider info try to determine the unique id of the external user (issued by the provider) the most
            // common claim type for that is the subclaim and the NameIdentifier depending on the external provider, some other claim type might
            // be used.
            Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                throw new InvalidOperationException("Unknown userid");

            string provider = result.Properties.Items["scheme"]
                              ?? throw new InvalidOperationException("Null scheme in authentication properties");
            string providerUserId = userIdClaim.Value;

            // Find external user:
            TestUser? user = _users.FindByExternalProvider(provider, providerUserId);

            if(user is null)
            {
                // This might be where you might initiate a custom workflow for user registration in this sample we don't show how that would be done,
                // as our sample implementation simply auto-provisions new external user.
                // Remove the user id claim so we don't include it as an extra claim if/when we provision the user.
                List<Claim> claims = externalUser.Claims.ToList();

                claims.Remove(userIdClaim);
                user = _users.AutoProvisionUser(provider, providerUserId, claims.ToList());
            }

            // This allows us to collect any additional claims or properties for the specific protocols used and store them in the local auth cookie.
            // This is typically used to store data needed for sign out from those protocols.
            List<Claim> additionalLocalClaims = [];
            AuthenticationProperties localSignInProps = new AuthenticationProperties();

            CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

            // Issues authentication cookie for user:
            IdentityServerUser isuser = new IdentityServerUser(user.SubjectId)
            {
                DisplayName = user.Username,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // Deletes temporary cookie used during external authentication:
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // Retrieves return URL:
            string returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // Checks if external login is in the context of an OIDC request:
            AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            await _events.RaiseAsync(
                new UserLoginSuccessEvent(
                    provider,
                    providerUserId,
                    user.SubjectId,
                    user.Username,
                    true, context?.Client.ClientId)
            );

            Telemetry.Metrics.UserLogin(context?.Client.ClientId, provider!);

            if(context is not null)
            {
                if(context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        // If the external login is OIDC-based, there are certain things we need to preserve to make logout work this will be different for WS-Fed,
        // SAML2p or other protocols.
        private static void CaptureExternalLoginContext(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps
        )
        {
            ArgumentNullException.ThrowIfNull(externalResult.Principal, nameof(externalResult.Principal));

            // Capture the idp used to login, so the session knows where the user came from:
            localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties?.Items["scheme"] ?? "unknown identity provider"));

            // If the external system sent a session id claim, copy it over so we can use it for a single sign-out:
            Claim? sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);

            if(sid is not null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));

            // If the external provider issued an id_token, we'll keep it for sign-out:
            string? idToken = externalResult.Properties?.GetTokenValue("id_token");

            if(idToken is not null) localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
        }
    }
}
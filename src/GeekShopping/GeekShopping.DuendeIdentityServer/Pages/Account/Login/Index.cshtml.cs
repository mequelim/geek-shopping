using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using GeekShopping.DuendeIdentityServer.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GeekShopping.DuendeIdentityServer.Pages.Account.Login
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;

        public ViewModel View { get; set; } = null!;

        [BindProperty] public InputModel Input { get; set; } = null!;

        public Index(
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            IEventService events,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string? returnUrl)
        {
            await BuildModelAsync(returnUrl);

            if(View.IsExternalLoginOnly)
            {
                // we only have one option for logging in, and it's an external provider
                return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // Check if we are in the context of an authorization request:
            AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            // The user clicked the "cancel" button:
            if(Input.Button != "login")
            {
                if(context is not null)
                {
                    // This "can't happen", because if the ReturnUrl was null, then the context would be null
                    ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                    // If the user cancels, send a result back into IdentityServer as if they denied the consent (even if this client does not
                    // require consent).
                    // This will send back access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // We can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null:
                    if(context.IsNativeClient())
                    {
                        // The client is native, so this change in how to return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    return Redirect(Input.ReturnUrl ?? "~/");
                }
                else
                {
                    // Since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if(ModelState.IsValid)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(
                    userName: Input.Username ?? string.Empty,
                    password: Input.Password ?? string.Empty,
                    isPersistent: Input.RememberLogin,
                    lockoutOnFailure: false
                );

                if(result.Succeeded)
                {
                    ApplicationUser? user = await _userManager.FindByNameAsync(Input.Username ?? string.Empty);

                    await _events.RaiseAsync(new UserLoginSuccessEvent(
                        username: user?.UserName,
                        subjectId: user?.Id,
                        name: user?.UserName,
                        clientId: context?.Client.ClientId
                    ));
                    Telemetry.Metrics.UserLogin(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider);

                    if(context != null)
                    {
                        // This "can't happen", because if the ReturnUrl was null, then the context would be null
                        ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                        if(context.IsNativeClient())
                        {
                            // The client is native, so this change in how to return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl);
                        }

                        // We can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl ?? "~/");
                    }

                    // request for a local page
                    if(Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else if(string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new ArgumentException("invalid return URL");
                    }
                }

                const string error = "invalid credentials";
                await _events.RaiseAsync(new UserLoginFailureEvent(Input.Username, error, clientId: context?.Client.ClientId));

                Telemetry.Metrics.UserLoginFailure(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider, error);
                ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            await BuildModelAsync(Input.ReturnUrl);
            return Page();
        }

        private async Task BuildModelAsync(string? returnUrl)
        {
            Input = new InputModel { ReturnUrl = returnUrl };

            AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if(context?.IdP is not null)
            {
                AuthenticationScheme? scheme = await _schemeProvider.GetSchemeAsync(context.IdP);

                if(scheme is not null)
                {
                    bool local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                    // This is meant to short-circuit the UI and only trigger the one external IdP:
                    View = new ViewModel { EnableLocalLogin = local, };

                    Input.Username = context.LoginHint;

                    if(!local)
                    {
                        View.ExternalProviders = [new ViewModel.ExternalProvider(authenticationScheme: context.IdP, displayName: scheme.DisplayName)];
                    }
                }

                return;
            }

            IEnumerable<AuthenticationScheme> schemes = await _schemeProvider.GetAllSchemesAsync();

            List<ViewModel.ExternalProvider> providers = schemes
                .Where((x) => x.DisplayName != null)
                .Select((x) => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Name,
                    displayName: x.DisplayName ?? x.Name
                )).ToList();

            IEnumerable<ViewModel.ExternalProvider> dynamicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
                .Where((x) => x.Enabled)
                .Select((x) => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Scheme,
                    displayName: x.DisplayName ?? x.Scheme
                ));

            providers.AddRange(dynamicSchemes);


            bool allowLocal = true;
            Client? client = context?.Client;

            if(client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if((!client.IdentityProviderRestrictions.Equals(null)) && (!client.IdentityProviderRestrictions.Count.Equals(0)))
                {
                    providers = providers
                        .Where((provider) => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme))
                        .ToList();
                }
            }

            View = new ViewModel
            {
                AllowRememberLogin = LoginOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
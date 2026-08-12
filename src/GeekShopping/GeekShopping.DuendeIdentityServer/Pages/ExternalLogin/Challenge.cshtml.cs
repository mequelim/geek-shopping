using Duende.IdentityServer.Services;
using GeekShopping.DuendeIdentityServer.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeekShopping.DuendeIdentityServer.Pages.ExternalLogin
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class Challenge : PageModel
    {
        private readonly IIdentityServerInteractionService _interactionService;

        public Challenge(IIdentityServerInteractionService interactionService) => _interactionService = interactionService;

        public IActionResult OnGet(string scheme, string? returnUrl)
        {
            if(string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // Abort on incorrect returnUrl - it is neither a local url nor a valid OIDC url:
            if((Url.IsLocalUrl(returnUrl).Equals(false)) && (_interactionService.IsValidReturnUrl(returnUrl).Equals(false)))
            {
                throw new ArgumentException("invalid return URL");  // User might have clicked on a malicious link - should be logged.
            }

            // Starts the challenge and roundtrip the return URL and scheme:
            AuthenticationProperties props = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/externallogin/callback"),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme }
                }
            };

            return Challenge(props, scheme);
        }
    }
}
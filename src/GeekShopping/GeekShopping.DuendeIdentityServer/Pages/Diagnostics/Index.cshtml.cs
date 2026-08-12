using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.DuendeIdentityServer.Pages.Diagnostics
{
    [SecurityHeaders]
    [Authorize]
    public class Index : PageModel
    {
        public ViewModel View { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            // Replaces with an authorization policy check:
            if(HttpContext.Connection.IsRemote()) return NotFound();

            View = new ViewModel(await HttpContext.AuthenticateAsync());

            return Page();
        }
    }
}
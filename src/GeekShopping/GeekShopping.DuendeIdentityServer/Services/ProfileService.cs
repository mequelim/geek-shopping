using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.DuendeIdentityServer.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.DuendeIdentityServer.Services
{
    public abstract class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext profileDataContext)
        {
            ApplicationUser? user = await userManager.GetUserAsync(profileDataContext.Subject);
            IList<string> roles = await userManager.GetRolesAsync(user!);
            List<Claim> claims = profileDataContext.IssuedClaims;

            claims.AddRange(
                roles.Select((role) => new Claim(JwtClaimTypes.Role, role))
            );
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            ApplicationUser? user = await userManager.GetUserAsync(context.Subject);
            context.IsActive = user is not null;
        }
    }
}
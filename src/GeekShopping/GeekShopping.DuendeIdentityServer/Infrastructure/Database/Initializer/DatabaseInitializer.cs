using GeekShopping.DuendeIdentityServer.Configuration.IdentityServer;
using GeekShopping.DuendeIdentityServer.Infrastructure.Database.Initializer.Interfaces;
using GeekShopping.DuendeIdentityServer.Model;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.DuendeIdentityServer.Infrastructure.Database.Initializer
{
    public class DatabaseInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IDatabaseInitializer
    {
        // Constructor:

        // Method:
        /// <summary>
        /// Initializes the database by creating the necessary roles and users.
        /// This method ensures the default administrative and client roles are present in the system.
        /// Additionally, it creates default admin and client users with associated claims, roles, and credentials if they do not already exist.
        /// </summary>
        public void Initialize()
        {
            if(roleManager.FindByNameAsync(IdentityServerConfiguration.Admin).Result is not null) return;

            roleManager
                .CreateAsync(new IdentityRole(IdentityServerConfiguration.Admin))
                .GetAwaiter()
                .GetResult();

            roleManager
                .CreateAsync(new IdentityRole(IdentityServerConfiguration.Client))
                .GetAwaiter()
                .GetResult();

            //? Admin:
            ApplicationUser admin = new()
            {
                UserName = "miquelin",
                Email = "pm.miquelin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (41) 98423-0550",
                PhoneNumberConfirmed = true,
                FirstName = "Pedro",
                LastName = "Miquelin"
            };

            userManager
                .CreateAsync(admin, "Pedr@2002")
                .GetAwaiter()
                .GetResult();

            userManager
                .AddToRoleAsync(admin, IdentityServerConfiguration.Admin)
                .GetAwaiter()
                .GetResult();

            /* IdentityResult adminClaims = userManager.AddClaimsAsync(admin, [
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.Role, IdentityServerConfiguration.Admin)
            ]).Result; */

            //? Client:
            ApplicationUser client = new()
            {
                UserName = "miquelin-client",
                Email = "pm.miquelin.client@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (41) 98423-0055",
                PhoneNumberConfirmed = true,
                FirstName = "Pedro",
                LastName = "Miquelin (Client)"
            };

            userManager
                .CreateAsync(client, "Pedr@2002")
                .GetAwaiter()
                .GetResult();

            userManager
                .AddToRoleAsync(client, IdentityServerConfiguration.Admin)
                .GetAwaiter()
                .GetResult();

            /* IdentityResult clientClaims = userManager.AddClaimsAsync(client, [
                new Claim(JwtClaimTypes.FamilyName, client.LastName),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new Claim(JwtClaimTypes.Role, IdentityServerConfiguration.Admin)
            ]).Result; */
        }
    }
}
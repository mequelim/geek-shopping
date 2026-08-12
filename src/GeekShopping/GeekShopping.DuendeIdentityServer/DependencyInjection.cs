using GeekShopping.DuendeIdentityServer.Configuration.IdentityServer;
using GeekShopping.DuendeIdentityServer.Infrastructure.Database;
using GeekShopping.DuendeIdentityServer.Infrastructure.Database.Initializer;
using GeekShopping.DuendeIdentityServer.Infrastructure.Database.Initializer.Interfaces;
using GeekShopping.DuendeIdentityServer.Model;
using GeekShopping.DuendeIdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeekShopping.DuendeIdentityServer
{
    public static class DependencyInjection
    {
        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Configures the application's UI services, enabling support for Razor Pages and MVC views.
            /// </summary>
            /// <remarks>
            /// This method registers Razor Pages and MVC controllers with views in the application's service collection, which are required for the
            /// Duende IdentityServer user interface to function properly.
            /// Call this method during the application startup to ensure the UI services are correctly set up and ready for use.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder AddUiServices()
            {
                //* 1. Adding support for Razor Pages (necessary for the Duende UI):
                builder.Services.AddRazorPages();
                builder.Services.AddControllersWithViews();

                return builder;
            }

            /// <summary>
            /// Configures the IdentityServer services required for the application.
            /// </summary>
            /// <remarks>
            /// This method sets up Identity services, including user and role management, token providers, and database initialization.
            /// It also registers custom implementations for user claims principal factory and database initializer.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder ConfigureIdentityServerServices()
            {
                builder.Services
                    .AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

                builder.Services
                    .AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();

                builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

                return builder;
            }

            /// <summary>
            /// Configures IdentityServer with the specified settings, including event handling, static audience claim emission, and developer signing credentials. 
            /// Additionally, it registers in-memory API resources and a custom profile service.
            /// </summary>
            /// <remarks>
            /// This method disables key management and enables event raising for error, success, information, and failure events. It also sets up the <see cref="ProfileService"/> for handling
            /// user profile data.
            /// </remarks>
            /// <returns>An <see cref="IIdentityServerBuilder"/> instance for further configuration of IdentityServer.</returns>
            public IIdentityServerBuilder AddIdentityServerConfiguration()
            {
                return builder.Services
                    .AddIdentityServer((options) =>
                    {
                        options.KeyManagement.Enabled = false;
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.EmitStaticAudienceClaim = true;
                    })
                    .AddProfileService<ProfileService>()  // ← volta o ProfileService
                    .AddDeveloperSigningCredential()
                    .AddInMemoryApiResources(IdentityServerConfiguration.ApiResources);
            }

            /// <summary>
            /// Configures the application's database services, setting up the Entity Framework context with PostgreSQL.
            /// </summary>
            /// <remarks>
            /// This method establishes the database connection string based on the application's configuration, 
            /// including support for Docker environments. It registers the <see cref="AppDbContext"/> with the dependency injection container, using PostgreSQL as the database provider.
            /// Call this method during the application startup to ensure the database services are properly configured.
            /// </remarks>
            public void AddDatabaseServices()
            {
                string? baseConnection = builder.Configuration.GetConnectionString("DefaultPostgresConnection");
                bool isDocker = builder.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");
                string host = (isDocker) ? "postgres" : "localhost";
                string finalConnectionString = $"Host={host};Port={5432};{baseConnection}";

                builder.Services.AddDbContext<AppDbContext>((options) =>
                {
                    options.UseNpgsql(
                        finalConnectionString,
                        (npgsqlOptions) => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName)
                    );
                });
            }
        }

        extension(WebApplication application)
        {
            /// <summary>
            /// Configures the HTTP request pipeline and sets up the necessary middleware for the application.
            /// </summary>
            /// <remarks>
            /// This method is responsible for configuring the application's middleware pipeline, including setting up exception handling,
            /// serving static files, enabling HTTPS redirection, and routing.
            /// It also integrates Duende IdentityServer for authentication and maps controller routes as well as razor page routes.
            /// This setup ensures the application is correctly configured to handle HTTP requests and manage user authentication through IdentityServer.
            /// Call this method during application startup to ensure that the middleware pipeline is configured properly.
            /// </remarks>
            /// <returns>The same <see cref="WebApplication"/> instance, allowing for method chaining.</returns>
            public WebApplication UseApi()
            {
                //* Configures the HTTP request pipeline:
                if(!application.Environment.IsDevelopment())
                {
                    application.UseExceptionHandler("/Home/Error");
                    application.UseHsts();
                    application.UseHttpsRedirection();
                }

                //? 3. Ensuring that static files (CSS/JS from the Duende wwwroot folder) are served:
                application.UseStaticFiles();
                application.UseRouting();

                //! 4. IMPORTANT: The IdentityServer takes control of authentication here:
                //? It MUST be between UseRouting and UseAuthorization.
                application.UseIdentityServer();
                application.UseAuthorization();
                application.MapStaticAssets();
                application.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                ).WithStaticAssets();

                //? 5. Mapping the Razor Pages routes to the Duende screens (Login, Logout, etc.) to work:
                application.MapRazorPages();

                return application;
            }

            /// <summary>
            /// Initializes the database for the application during startup.
            /// </summary>
            /// <remarks>
            /// This method creates a service scope to resolve the required <see cref="IDatabaseInitializer"/> service and invokes its
            /// <c>Initialize</c> method.
            /// It ensures that the database is configured and seeded appropriately based on the application's requirements.
            /// Call this method during the application startup to prepare the database for use and to ensure it is in the expected state.
            /// </remarks>
            public void InitializeDatabase()
            {
                using IServiceScope scope = application.Services.CreateScope();

                IDatabaseInitializer databaseInitializer = scope.ServiceProvider .GetRequiredService<IDatabaseInitializer>();
                databaseInitializer.Initialize();
            }
        }
    }
}
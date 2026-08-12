using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

namespace GeekShopping.Web
{
    public static class DependencyInjection
    {
        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Adds required services to the dependency injection container, including controllers with views and HTTP client configuration for the product
            /// service.
            /// </summary>
            public WebApplicationBuilder AddServices()
            {
                builder.Services.AddControllersWithViews();
                builder.Services.AddHttpClient<ICartService, CartService>((config) =>
                {
                    config.BaseAddress = new Uri(builder.Configuration["ServicesUrls:CartAPI"]!);
                });
                builder.Services.AddHttpClient<ICouponService, CouponService>((config) =>
                {
                    config.BaseAddress = new Uri(builder.Configuration["ServicesUrls:DiscountCouponAPI"]!);
                });
                builder.Services.AddHttpClient<IProductService, ProductService>((config) =>
                {
                    config.BaseAddress = new Uri(builder.Configuration["ServicesUrls:ProductAPI"]!);
                });

                return builder;
            }

            /// <summary>
            /// Configures authentication and authorization for the application using cookie-based authentication and OpenID Connect.
            /// Sets up schemes, options, and policies for managing user authentication, token handling, and claims mapping.
            /// The configuration includes integration with an external identity server and specifies client credentials and claims mapping for role-based access.
            /// </summary>
            public void ConfigureAuthorization()
            {
                builder.Services
                    .AddAuthentication((options) =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    })
                    .AddCookie(
                        authenticationScheme: "Cookies",
                        configureOptions: (config) =>
                        {
                            config.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                            config.AccessDeniedPath = "/Home/Index";
                        }
                    )
                    .AddOpenIdConnect(
                        authenticationScheme: "oidc",
                        configureOptions: (options) =>
                        {
                            options.Authority = builder.Configuration["ServicesUrls:IdentityServer"];
                            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;
                            options.GetClaimsFromUserInfoEndpoint = true;
                            options.ClientId = "geek_shopping";
                            options.ClientSecret = builder.Configuration["Secrets:ClientSecrets"];
                            options.ResponseType = "code";
                            options.TokenValidationParameters.NameClaimType = "name";
                            options.TokenValidationParameters.RoleClaimType = "role";
                            options.Scope.Add("geek_shopping");
                            options.Scope.Add("role");
                            options.MapInboundClaims = false;
                            options.ClaimActions.MapJsonKey("role", "role", "role");
                            options.ClaimActions.MapJsonKey("sub", "sub", "sub");
                            options.SaveTokens = true;
                        }
                    );
            }
        }

        /// <summary>
        /// Configures the middleware pipeline for the web application, including routing, exception handling, HTTPS redirection static assets, authorization, and default controller route mapping.
        /// </summary>
        /// <param name="application">An instance of <see cref="WebApplication"/> to which the middleware components will be applied.</param>
        public static void UseApi(this WebApplication application)
        {
            if(!application.Environment.IsDevelopment())
            {
                application.UseExceptionHandler("/Home/Error");
                application.UseHsts();
            }

            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();
            application.MapStaticAssets();
            application.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            ).WithStaticAssets();
        }
    }
}
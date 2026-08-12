using GeekShopping.GatewayAPI.Shared.Converters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.GatewayAPI
{
    /// <summary>
    /// Provides extension methods for configuring application services related to dependency injection, including database services, API services, and other middleware configurations.
    /// </summary>
    public static class DependencyInjection
    {
        /// <param name="builder">
        /// The <see cref="WebApplicationBuilder"/> instance used to configure services.
        /// Must not be null.
        /// </param>
        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Adds and configures API-related services to the application's service collection.
            /// </summary>
            /// <remarks>
            /// This method sets up JSON serialization options, including camel case naming policies for properties and dictionary keys, and ignoring null values during
            /// serialization. Additionally, it registers the necessary converters to handle DateOnly and TimeOnly data types correctly.
            /// The method also adds services required for controllers and API endpoint exploration.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder AddApiServices()
            {
                builder.Services.Configure<JsonOptions>((options) =>
                {
                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                    //note: Crucial for DateOnly/TimeOnly to function correctly:
                    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.SerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                });

                builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                return builder;
            }

            /// <summary>
            /// Configures authentication services for the application, including JWT bearer authentication.
            /// </summary>
            /// <remarks>
            /// This method sets up authentication schemes using JWT bearer tokens, connecting to an identity server specified in the application's configuration to
            /// validate incoming tokens.
            /// It disables audience validation and configures the role claim type for proper role processing.
            /// Additionally, it wires up event handlers for logging authentication failures and token validation outcomes.
            /// The method also adds Ocelot services for API gateway functionality.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, facilitating method chaining.</returns>
            public WebApplicationBuilder ConfigureAuthentication()
            {
                builder.Services
                    .AddAuthentication("Bearer")
                    .AddJwtBearer((options) =>
                    {
                        options.Authority = builder.Configuration["ServicesUrls:IdentityServer"];
                        options.MapInboundClaims = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            RoleClaimType = "role"
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = (context) =>
                            {
                                Console.WriteLine($">>> JWT FAILED: {context.Exception.Message}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = (context) =>
                            {
                                Console.WriteLine($">>> JWT VALID, claims: {
                                    string.Join(", ", context.Principal!.Claims.Select((ctx) => $"{ctx.Type}={ctx.Value}"))
                                }");
                                return Task.CompletedTask;
                            }
                        };
                    });
                builder.Services.AddOcelot();

                return builder;
            }

            /// <summary>
            /// Configures the authorization policies and services for the application.
            /// </summary>
            /// <remarks>
            /// This method sets up the default authorization policy to require authenticated users and defines a custom policy named "ApiScope".
            /// The "ApiScope" policy ensures that requests include a specific claim with the key "scope" and the value "geek_shopping", making it suitable for securing
            /// API endpoints with granular
            /// access control.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder ConfigureAuthorization()
            {
                builder.Services.AddAuthorization((options) =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.AddPolicy(
                        name: "ApiScope",
                        configurePolicy: (policy) =>
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireClaim("scope", "geek_shopping");
                        }
                    );
                });

                return builder;
            }

            /// <summary>
            /// Configures the application's CORS (Cross-Origin Resource Sharing) policy to allow unrestricted access from any origin, enabling development and testing
            /// scenarios.
            /// </summary>
            /// <remarks>
            /// This method sets up a default CORS policy named "DefaultCors" within the application's service collection.
            /// The policy permits requests from any origin, with any header, and any HTTP method.
            /// Use this policy to facilitate communication between client and server during development where cross-origin requests are necessary.
            /// Ensure proper configuration in production environments to restrict origins and secure communication.
            /// </remarks>
            public void AddCorsPolicy()
            {
                builder.Services.AddCors((options) =>
                {
                    options.AddPolicy("DefaultCors", (policy) =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            }
        }

        /// <summary>
        /// Configures the application to use API middleware necessary for request routing and response handling.
        /// </summary>
        /// <remarks>
        /// This method integrates and activates the Ocelot middleware for the API gateway functionality, enabling it to manage request routing, aggregation, and other
        /// features for downstream services.
        /// </remarks>
        /// <param name="application">The <see cref="WebApplication"/> instance to configure with the API middleware.</param>
        /// <returns>The configured <see cref="WebApplication"/> instance, enabling method chaining.</returns>
        public static WebApplication UseApi(this WebApplication application)
        {
            application.UseOcelot();

            return application;
        }
    }
}
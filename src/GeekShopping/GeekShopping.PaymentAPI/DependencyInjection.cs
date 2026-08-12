using GeekShopping.PaymentAPI.RabbitMQ.PaymentConsumer;
using GeekShopping.PaymentAPI.RabbitMQ.Sender;
using GeekShopping.PaymentAPI.RabbitMQ.Sender.Interface;
using GeekShopping.PaymentAPI.Shared.Converters;
using GeekShopping.PaymentAPI.Shared.Extensions;
using GeekShopping.PaymentProcessor.Class;
using GeekShopping.PaymentProcessor.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace GeekShopping.PaymentAPI
{
    /// <summary>
    /// Provides extension methods for configuring application services related to dependency injection, including database services, API services, and other middleware
    /// configurations.
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
            /// Adds and configures the API services required for the application.
            /// </summary>
            /// <remarks>
            /// This method registers and configures essential services for the API, including JSON serializer options, AutoMapper, repositories, controllers, and OpenAPI/Swagger for API documentation.
            /// It sets JSON serialization policies to handle naming conventions and adds specific converters for custom data types such as enums, DateOnly, and TimeOnly.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder AddApiServices()
            {
                builder.Services.Configure<JsonOptions>((options) =>
                {
                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                    //! Crucial for DateOnly/TimeOnly to function correctly:
                    // options.SerializerOptions.Converters.Add(new DecimalJsonConverter());
                    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.SerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                });

                builder.Services.AddSingleton<IProcessorPayment, ProcessPayment>();
                builder.Services.AddSingleton<IRabbitMqMessageSender, RabbitMqMessageSender>();
                builder.Services.AddHostedService<RabbitMqPaymentConsumer>();
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddOpenApi((options) =>
                {
                    options.AddDocumentTransformer(async (document, context, cancellationToken) =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        IConfiguration config = context.ApplicationServices.GetRequiredService<IConfiguration>();
                        string authUrl = config["ServicesUrls:IdentityServer"]
                                         ?? throw new InvalidOperationException("IdentityServer URL is missing!");

                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                        OpenApiSecurityScheme scheme = new()
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = $"Obtain the token at `POST {authUrl}/connect/token` (access_token field) via Postman!"
                        };

                        document.Components.SecuritySchemes["Bearer"] = scheme;

                        OpenApiSecurityRequirement requirement = new()
                        {
                            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                        };

                        document.Security = [requirement];

                        await Task.CompletedTask;
                    });
                });

                return builder;
            }

            /// <summary>
            /// Configures authentication services for the application.
            /// </summary>
            /// <remarks>
            /// This method sets up JWT Bearer authentication for the application.
            /// It configures the authority for token validation using the Identity Server URL specified in the application's configuration.
            /// Additionally, it customizes token validation parameters, such as disabling audience validation and setting the claim type for roles.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
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
                                    string.Join(", ", context.Principal!.Claims.Select(ctx => $"{ctx.Type}={ctx.Value}"))
                                }");
                                return Task.CompletedTask;
                            }
                        };
                    });

                return builder;
            }

            /// <summary>
            /// Configures the authorization policies and services for the application.
            /// </summary>
            /// <remarks>
            /// This method sets up the default authorization policy to require authenticated users and defines a custom policy named "ApiScope".
            /// The "ApiScope" policy ensures that requests include a specific claim with the key "scope" and the value "geek_shopping", making it suitable for securing API endpoints with granular
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
            /// Configures the application's CORS (Cross-Origin Resource Sharing) policy to allow unrestricted access from any origin, enabling development and testing scenarios.
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
        /// Configures the application's middleware pipeline and localization settings.
        /// </summary>
        /// <param name="application">The <see cref="WebApplication"/> instance to configure.</param>
        /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
        /// <remarks>
        /// This method performs the following configurations:
        /// - Sets up request localization with supported cultures.
        /// - Enables HTTPS redirection.
        /// - Configures routing for the application.
        /// - Applies the default CORS policy named "DefaultCors".
        /// - Enables authentication and authorization middleware.
        /// - Configures additional custom middleware components using <see cref="MiddlewareExtensions.UseMiddlewares"/>.
        /// - Maps the application's controllers to endpoints.
        /// This method is typically called during application startup to ensure the middleware pipeline is properly configured.
        /// </remarks>
        public static WebApplication UseApi(this WebApplication application)
        {
            string[] supportedCultures = ["pt-BR"];

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            application.UseRequestLocalization(localizationOptions);
            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseCors("DefaultCors");
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseMiddlewares();
            application.MapControllers();

            return application;
        }
    }
}
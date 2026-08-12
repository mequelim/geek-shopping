using AutoMapper;
using GeekShopping.ProductAPI.Configs.Mappings;
using GeekShopping.ProductAPI.Infrastructure.Database;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Repository.Interfaces;
using GeekShopping.ProductAPI.Shared.Converters;
using GeekShopping.ProductAPI.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.ProductAPI
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

                //* Mapper:
                IMapper mapper = MappingConfiguration.RegisterMappings().CreateMapper();

                builder.Services.AddSingleton(mapper);
                builder.Services.AddAutoMapper(
                    _ => { },
                    typeof(Program)
                );

                //* Repositories:
                builder.Services.AddScoped<IProductRepository, ProductRepository>();

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
            /// The "ApiScope" policy ensures that requests include a specific claim with the key "scope" and the value "geek_shopping", making it suitable for securing API
            /// endpoints with granular access control.
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
            /// Configures the application's database services, including the setup of the Entity Framework database context and connection string
            /// for PostgreSQL.
            /// </summary>
            /// <remarks>
            /// This method registers the <see cref="AppDbContext"/> with the dependency injection container and configures it to use PostgreSQL as the database provider.
            /// The connection string is dynamically built based on the application's environment, accounting for whether it is running inside a Docker container.
            /// Call this method during the application startup to ensure the database services are properly registered and ready for use.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder AddDatabaseServices()
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
        /// Configures the application's middleware pipeline and API behavior.
        /// </summary>
        /// <remarks>
        /// This method sets up essential middleware and configurations for the application, including request localization, HTTPS redirection, routing, CORS policy, request
        /// handling, authentication, authorization, custom middlewares, and controller mapping.
        /// It also handles OpenAPI/Swagger configuration in the development environment.
        /// </remarks>
        /// <param name="application">The <see cref="WebApplication"/> instance to configure.</param>
        /// <returns>The configured <see cref="WebApplication"/> instance, allowing further modifications or starting the application.</returns>
        public static WebApplication UseApi(this WebApplication application)
        {
            string[] supportedCultures = ["pt-BR"];

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            if(application.Environment.IsDevelopment())
            {
                application.MapOpenApi();
                application.MapScalarApiReference((options) =>
                {
                    options.Authentication = new ScalarAuthenticationOptions
                    {
                        PreferredSecuritySchemes = ["Bearer"]
                    };
                    options.DarkMode = true;
                    options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Shell, ScalarClient.Httpie);
                    options.DefaultOpenAllTags = false;
                    options.DocumentDownloadType = DocumentDownloadType.Json;
                    options.EnabledClients = [ScalarClient.Httpie];
                    options.ExpandAllModelSections = false;
                    options.ExpandAllResponses = false;
                    options.HideDarkModeToggle = false;
                    options.HideModels = true;
                    options.HideTestRequestButton = false;
                    options.Layout = ScalarLayout.Modern;
                    options.OperationSorter = OperationSorter.Alpha; // or by HTTP method.
                    options.SchemaPropertyOrder = PropertyOrder.Preserve; // or alpha.
                    options.ShowDeveloperTools = DeveloperToolsVisibility.Localhost; // or always or never.
                    options.ShowOperationId = false;
                    options.ShowSidebar = true;
                    options.SortTagsAlphabetically(); // The same as `options.TagSorter = TagSorter.Alpha`.
                    options.Telemetry = true;
                    options.Theme = ScalarTheme.BluePlanet;
                    options.Title = "GeekShopping - ProductAPI";
                });
            }

            application.UseRequestLocalization(localizationOptions);
            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseCors("DefaultCors");
            application.Use(async (context, next) =>
            {
                if(context.Request.Path.StartsWithSegments("/api/Products/DeleteProduct"))
                {
                    string? auth = context.Request.Headers.Authorization;
                    Console.WriteLine($">>> AUTH HEADER: {auth?[..50]}...");
                }

                await next();
            });
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseMiddlewares();
            application.MapControllers();

            return application;
        }
    }
}
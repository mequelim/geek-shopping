using GeekShopping.DuendeIdentityServer;
using GeekShopping.DuendeIdentityServer.Configuration.IdentityServer;
using GeekShopping.DuendeIdentityServer.Model;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddUiServices()
    .ConfigureIdentityServerServices()
    .AddDatabaseServices();

IIdentityServerBuilder isBuilder = builder.AddIdentityServerConfiguration();

//? As this is a study project, the in-memory template configurations will be entered here:
isBuilder
    .AddInMemoryClients(IdentityServerConfiguration.GetClients(builder.Configuration))
    .AddInMemoryIdentityResources(IdentityServerConfiguration.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfiguration.ApiScopes);

//* Examples of what you will add here later:
// isBuilder.AddTestUsers(IdentityServerConfiguration.TestUsers);

isBuilder.AddAspNetIdentity<ApplicationUser>();

WebApplication application = builder.Build();
application
    .UseApi()
    .InitializeDatabase();
application.Run();
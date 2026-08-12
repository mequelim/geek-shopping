using GeekShopping.GatewayAPI;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .ConfigureAuthentication()
    .ConfigureAuthorization()
    .AddCorsPolicy();

WebApplication application = builder.Build();
application.UseApi();
await application.RunAsync();
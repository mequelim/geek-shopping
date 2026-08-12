using GeekShopping.DiscountCouponAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddDatabaseServices()
    .AddApiServices()
    .ConfigureAuthentication()
    .ConfigureAuthorization()
    .AddCorsPolicy();

WebApplication application = builder.Build();
application.UseApi();
await application.RunAsync();
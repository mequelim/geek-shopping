using GeekShopping.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder
    .AddServices()
    .ConfigureAuthorization();

WebApplication application = builder.Build();
application.UseApi();

await application.RunAsync();
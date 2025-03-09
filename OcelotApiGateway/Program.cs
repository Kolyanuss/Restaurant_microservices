using Ocelot.Middleware;
using OcelotApiGateway;
using JwtConfiguration;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration
//    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();
//builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCustomOcelot(builder.Configuration, "ocelot.json");

builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "ADMIN"));
});


var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();

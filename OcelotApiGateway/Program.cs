using Ocelot.Middleware;
using OcelotApiGateway;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration
//    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();
//builder.Services.AddOcelot(builder.Configuration);
//.AddConsul();

builder.Services.AddCustomOcelot(builder.Configuration, "ocelot.json");

var app = builder.Build();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();

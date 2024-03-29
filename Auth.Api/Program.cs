using Auth.Api;
using Auth.Api.Clients;
using Auth.Api.Models.DbContext;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");

var loggerUri = builder.Configuration.GetConnectionString("LogStashConnectionString");
var serviceName = builder.Configuration.GetSection("ServiceInfo")?["ServiceName"];

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.Enrich.WithProperty("ServiceName", serviceName);
    lc.WriteTo.Console(
            outputTemplate:
            "[{ServiceName} {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .ReadFrom.Configuration(ctx.Configuration);

    if (loggerUri is not null)
        lc.WriteTo.Http(loggerUri, null, httpClient: new CustomHttpClient());
});

builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
{
    configurationBuilder.AddJsonFile("appsettings.json", true, true);
    if (context.HostingEnvironment.EnvironmentName != "Production")
        configurationBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
});

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

startup.Configure(app, app.Environment, provider);


app.UseSerilogRequestLogging();

app.UseHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();


// Migrate latest database changes during startup
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationContext>();

    var migrateDbPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

    migrateDbPolicy.Execute(() =>
    {
        // Here is the migration executed
        dbContext.Database.Migrate();
    });
}

//app.UseHttpsRedirection();

app.Run();


Log.Information("Application is shutting down...");
Log.CloseAndFlush();


public partial class Program
{
}
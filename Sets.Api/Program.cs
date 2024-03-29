using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;
using Sets.Api.Common.Clients;
using Sets.Api.Extensions.DiExtensions;
using Sets.Api.Models.DbContext;
using ext = Sets.Api.Extensions.EnvironmentExtensions;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");

var loggerUri = builder.Configuration.GetConnectionString("LogStashConnectionString");

builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
{
    configurationBuilder.AddJsonFile("appsettings.json", true, true);
    if (ext.IsProduction())
    {
        var uri = new Uri(builder.Configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        configurationBuilder.AddAzureKeyVault(uri, new DefaultAzureCredential());
    }

    if (context.HostingEnvironment.EnvironmentName != "Production")
        configurationBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
});

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceName = builder.Configuration.GetSection("ServiceInfo")?["ServiceName"];

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.Enrich.WithProperty("ServiceName", serviceName);
    lc.WriteTo.Console(outputTemplate:
            "[{ServiceName} {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .ReadFrom.Configuration(ctx.Configuration);

    if (loggerUri is not null) lc.WriteTo.Http(loggerUri, null, httpClient: new CustomHttpClient());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

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

app.UseRouting();

app.UseHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


Log.Information("Application is shutting down...");
Log.CloseAndFlush();

public partial class Program
{
}
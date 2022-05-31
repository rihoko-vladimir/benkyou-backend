using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notification.Api.Extensions.ConfigurationExtensions;
using Notification.Api.Extensions.DIExtensions;
using Notification.Api.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
    var secretClient = new SecretClient(uri, new DefaultAzureCredential());
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });
    var emailConfiguration = configuration.GetEmailConfiguration();
    builder.Services.AddHealthChecks()
        .AddCheck("SmtpCheck", new PingHealthCheck(emailConfiguration.Server), tags: new List<string> {"Email"})
        .AddAzureKeyVault(uri, new DefaultAzureCredential(), options => { }, "Azure Key vault",
            HealthStatus.Unhealthy, new List<string> {"Azure Key Vault"});

    builder.Services.AddSingleton(logger);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEmailSender();
    builder.Services.AddConfiguredMassTransit(configuration, secretClient);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal("Unhandled exception: {Type} Message: {Message} Stacktrace: {Stacktrace}", e.GetType().FullName,
        e.Message, e.StackTrace);
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}
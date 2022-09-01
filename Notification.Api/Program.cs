using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Api.Extensions;
using Notification.Api.Extensions.DIExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });

    if (EnvironmentExtensions.IsProduction())
    {
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        builder.Configuration.AddAzureKeyVault(uri, new DefaultAzureCredential());
    }
    else
    {
        builder.Configuration.AddEnvironmentVariables(source => { source.Prefix = "APP_"; });
    }

    builder.Services.AddApplication(configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseRouting();
    
    app.UseHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.Run();
}
catch (Exception e)
{
    Log.Error("Unhandled exception: {Type} Message: {Message} Stacktrace: {Stacktrace}", e.GetType().FullName,
        e.Message, e.StackTrace);
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}
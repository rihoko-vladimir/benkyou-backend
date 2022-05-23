using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Api.Extensions.ConfigurationExtensions;
using Notification.Api.Extensions.DIExtensions;
using Notification.Api.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var loggerBuilder = new LoggerConfiguration()
    .WriteTo.Console();
if (builder.Environment.IsDevelopment()) loggerBuilder.MinimumLevel.Debug();
else loggerBuilder.MinimumLevel.Information();
var logger = loggerBuilder.CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    builder.Logging.AddSerilog(logger);

    var emailConfiguration = configuration.GetEmailConfiguration();
    builder.Services.AddHealthChecks()
        .AddCheck("SmtpCheck", new PingHealthCheck(emailConfiguration.Server));

    builder.Services.AddSingleton(logger);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEmailSender();

    var app = builder.Build();

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

    app.MapDefaultControllerRoute();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal("Unhandled exception: {Type} Message: {Message} Stacktrace: {Stacktrace}", e.GetType().FullName, e.Message ,e.StackTrace);
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}

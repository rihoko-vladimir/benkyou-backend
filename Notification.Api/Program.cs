using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Api.Extensions;
using Notification.Api.Generators;
using Notification.Api.HealthChecks;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Logging.ClearProviders();
var loggerBuilder = new LoggerConfiguration()
    .WriteTo.Console();
if (builder.Environment.IsDevelopment())
    loggerBuilder.MinimumLevel.Debug();
else
    loggerBuilder.MinimumLevel.Information();
var logger = loggerBuilder.CreateLogger();
builder.Logging.AddSerilog(logger);

var emailConfiguration = configuration.GetEmailConfiguration();
builder.Services.AddHealthChecks()
    .AddCheck("SmtpCheck", new PingHealthCheck(emailConfiguration.Server));

builder.Services.AddSingleton(logger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmailTemplateGenerator, EmailTemplateGenerator>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

var app = builder.Build();

app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapDefaultControllerRoute();

app.Run();
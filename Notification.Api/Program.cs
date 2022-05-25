using HealthChecks.UI.Client;
using MassTransit;
using Messages.Contracts;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Api.Consumers;
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
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });
    var emailConfiguration = configuration.GetEmailConfiguration();
    builder.Services.AddHealthChecks()
        .AddCheck("SmtpCheck", new PingHealthCheck(emailConfiguration.Server));

    builder.Services.AddSingleton(logger);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEmailSender();
    builder.Services.AddMassTransit(configurator =>
    {
        configurator.AddConsumer<SendEmailCodeConsumer>();
        configurator.AddConsumer<SendPasswordResetConsumer>();
        configurator.UsingRabbitMq((context, factoryConfigurator) =>
        {
            factoryConfigurator.ReceiveEndpoint(QueueNames.EmailConfirmationQueue,
                endpointConfigurator => { endpointConfigurator.ConfigureConsumer<SendEmailCodeConsumer>(context); });
            factoryConfigurator.ReceiveEndpoint(QueueNames.PasswordResetQueue,
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<SendPasswordResetConsumer>(context);
                });
            factoryConfigurator.Host("rabbitmq");
        });
    });

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

    app.MapDefaultControllerRoute();

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
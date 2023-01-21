using Health.Api.Clients;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    var loggerUri = builder.Configuration.GetConnectionString("LogStashConnectionString");
    var serviceName = builder.Configuration.GetSection("ServiceInfo")?["ServiceName"];

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.Enrich.WithProperty("ServiceName", serviceName);
        lc.WriteTo.Console(outputTemplate:
                "[{ServiceName} {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .ReadFrom.Configuration(ctx.Configuration);

        if (loggerUri is not null) lc.WriteTo.Http(loggerUri, null, httpClient: new CustomHttpClient());
    });
    builder.Services.AddHealthChecks();

    builder.Services.AddSingleton(logger);

    builder.Services.AddHealthChecksUI()
        .AddInMemoryStorage();

    var app = builder.Build();

    app.UseRouting();

//app.UseHttpsRedirection();
    app.UseEndpoints(routeBuilder => { routeBuilder.MapHealthChecksUI(options => options.UIPath = "/health-ui"); });

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
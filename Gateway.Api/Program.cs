using Gateway.Api.Clients;
using Gateway.Api.Extensions.JWTExtensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    var loggerUri = builder.Configuration.GetConnectionString("LogStashConnectionString");
    var serviceName = builder.Configuration.GetSection("ServiceInfo")?["ServiceName"];

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.Enrich.WithProperty("ServiceName", serviceName);
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);

        if (loggerUri is not null) lc.WriteTo.Http(loggerUri, null, httpClient: new CustomHttpClient());
    });

    builder.Configuration
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile("appsettings.Development.json", true, true)
        .AddJsonFile("ocelot.json")
        .AddJsonFile("ocelot.Development.json")
        .AddEnvironmentVariables();

    builder.Services.AddAuthentication()
        .AddJwtBearer("Jwt", options => { options.ConfigureJwtBearer(builder.Configuration); });

    builder.Services.AddOcelot();

    builder.Services.AddCors(builder =>
    {
        builder.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseCors();

    app.UseOcelot().Wait();

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
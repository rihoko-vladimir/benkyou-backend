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

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });
    
    builder.Configuration
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.Development.json", true, true)
        .AddJsonFile("ocelot.json")
        .AddJsonFile("ocelot.Development.json")
        .AddEnvironmentVariables();

    builder.Services.AddAuthentication()
        .AddJwtBearer("Jwt", options =>
        {
            options.ConfigureJwtBearer(builder.Configuration);
        });
    
    builder.Services.AddOcelot();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseOcelot().Wait();

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
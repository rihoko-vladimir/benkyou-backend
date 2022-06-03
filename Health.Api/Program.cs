using Serilog;

var builder = WebApplication.CreateBuilder(args);
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
    Log.Fatal("Unhandled exception: {Type} Message: {Message} Stacktrace: {Stacktrace}", e.GetType().FullName,
        e.Message, e.StackTrace);
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}
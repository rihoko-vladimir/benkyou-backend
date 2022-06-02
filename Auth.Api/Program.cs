using Serilog;

namespace Auth.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console().CreateLogger();
            Log.Logger = logger;

            Log.Information("Application is starting up...");

            await CreateHostBuilder(args).Build().RunAsync();
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
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog((ctx, lc) =>
            {
                lc.WriteTo.Console()
                    .ReadFrom.Configuration(ctx.Configuration);
            }).ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", true, true);
                if (context.HostingEnvironment.EnvironmentName != "Production")
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
            });
    }
}
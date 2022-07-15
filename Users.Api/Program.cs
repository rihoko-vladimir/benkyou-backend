using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using Users.Api.Extensions.DiExtensions;
using ext = Users.Api.Extensions.EnvironmentExtensions;


var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();
Log.Logger = logger;
Log.Information("Application is starting up...");
try
{
    builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        configurationBuilder.AddJsonFile("appsettings.json", true, true);
        if (ext.IsProduction())
        {
            var uri = new Uri(builder.Configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
            configurationBuilder.AddAzureKeyVault(uri, new DefaultAzureCredential());
        }

        if (context.HostingEnvironment.EnvironmentName != "Production")
            configurationBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json");
    });

    builder.Services.AddControllers(options =>
        {
            var supportedTypes = options.InputFormatters
                .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>()
                .Single().SupportedMediaTypes;
            supportedTypes.Add("image/jpeg");
            supportedTypes.Add("image/png");
        })
        .AddNewtonsoftJson();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddApplication(builder.Configuration);

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration);
    });

    var app = builder.Build();

    if (!ext.IsProduction())
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var desc in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"../swagger/{desc.GroupName}/swagger.json", desc.ApiVersion.ToString());
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(DocExpansion.None);
            }
        });
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    app.UseHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.UseAuthentication();

//app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

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
using Azure.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using Users.Api.Extensions.DiExtensions;
using ext = Users.Api.Extensions.EnvironmentExtensions;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers()
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
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using Auth.Api.Extensions.DIExtensions;
using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using ext = Auth.Api.Extensions.EnvironmentExtensions;

namespace Auth.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddConfiguration(configuration);
        if (ext.IsProduction()) configurationBuilder.AddAzureKeyVault(uri, new DefaultAzureCredential());

        _configuration = configurationBuilder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication(_configuration);
        services.AddControllers();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (!ext.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var desc in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"../swagger/{desc.GroupName}/swagger.json", desc.ApiVersion.ToString());
                    options.DefaultModelsExpandDepth(-1);
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
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

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSerilogRequestLogging();


        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}
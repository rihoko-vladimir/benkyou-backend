using Auth.Api.Extensions.DIExtensions;
using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSerilogRequestLogging();


        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}
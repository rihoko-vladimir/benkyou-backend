using Auth.Api.Extensions.DIExtensions;
using Auth.Api.Extensions.JWTExtensions;
using Auth.Api.Models.DbContext;
using Azure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Auth.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        var uri = new Uri(configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddConfiguration(configuration);
        configurationBuilder.AddAzureKeyVault(uri, new DefaultAzureCredential());
        _configuration = configurationBuilder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var uri = new Uri(_configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        services.AddApplication(_configuration);
        services.AddConfiguredMassTransit(_configuration);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(_configuration); });
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationContext>("Users database", tags: new List<string> {"Database"})
            .AddAzureKeyVault(uri, new DefaultAzureCredential(), _ => { }, "Azure Key vault",
                HealthStatus.Unhealthy, new List<string> {"Azure Key Vault"});
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

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}
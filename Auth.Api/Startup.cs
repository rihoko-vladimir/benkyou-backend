using Auth.Api.Extensions.DIExtensions;
using Auth.Api.Extensions.JWTExtensions;
using Auth.Api.Models.DbContext;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication();
        var uri = new Uri(_configuration.GetSection("KeyVault").GetValue<string>("VaultUri"));
        var secretClient = new SecretClient(uri, new DefaultAzureCredential()); 
        services.AddConfiguredMassTransit(_configuration,secretClient);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.ConfigureJwtBearer(_configuration); });
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(_configuration.GetConnectionString("SqlServerConnectionString") ?? "",
                builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationContext>("Users database", tags: new List<string> {"Database"});
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
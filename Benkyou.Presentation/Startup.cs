using System;
using System.Text;
using Benkyou.Domain.Database;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Extensions;
using Benkyou.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Benkyou_backend;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddJwtProperties(configuration);
        var provider = services.BuildServiceProvider();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnectionString") ?? "");
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            var jwtParams = provider.GetService<JwtProperties>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtParams!.AccessSecret));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtParams!.Issuer,
                ValidAudience = jwtParams!.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddIdentityCore<User>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}
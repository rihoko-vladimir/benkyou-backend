using System;
using System.Text;
using System.Threading.Tasks;
using Benkyou.Application.Common;
using Benkyou.Application.Repositories;
using Benkyou.Application.Services.Common;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Database;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Extensions;
using Benkyou.Infrastructure.Generators;
using Benkyou.Infrastructure.Repositories;
using Benkyou.Infrastructure.Services;
using Benkyou.Infrastructure.TokenProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Benkyou_backend;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(builder =>
        {
            builder.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .SetIsOriginAllowed(s => s.Contains("localhost"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAccessTokenService, JwtAccessTokenService>();
        services.AddScoped<IRefreshTokenService, JwtRefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenValidationService, TokenValidationService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<ISetsRepository, SetsRepository>();
        services.AddScoped<IUserStatisticsRepository, UserStatisticsRepository>();
        services.AddScoped<IAvatarUploadService, AvatarUploadService>();
        services.AddScoped<ApplicationUnitOfWork>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(_configuration.GetConnectionString("SqlServerConnectionString") ?? "",
                builder =>
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            //options.LogTo(Console.WriteLine);
        });
        var jwtParams = services.AddJwtProperties(_configuration);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtParams.AccessSecret));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtParams.Issuer,
                ValidAudience = jwtParams.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };
            options.TokenValidationParameters = validationParameters;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access"];
                    return Task.CompletedTask;
                }
            };
        });
        services.AddIdentityCore<User>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddTokenProvider(TokenProviders.EmailCodeTokenProviderName,
            typeof(EmailCodeTokenProvider)).AddDefaultTokenProviders();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseAuthentication();

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}
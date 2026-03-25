using Domain.Models.Configurations;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LMS.API.Extensions;

public static class AuthServiceExtension
{
    //User Secrets Json
    //Important to have secretkey inside same key "JwtSettings" as used in appsettings.json for get both sections!!!!
    //{
    //     "password": "YourSecretPasswordHere",
    //     "JwtSettings": {
    //        "secretkey": "ThisMustBeReallyLong!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
    //        }
    //}
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration
                         .GetSection(JwtSettings.Section)
                         .Get<JwtSettings>()
                         ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey is missing or empty. Add it to appsettings (e.g. appsettings.Development.json), user secrets: JwtSettings:SecretKey, or environment variables.");
        }

        services.AddOptions<JwtSettings>()
                        .Bind(configuration.GetSection(JwtSettings.Section))
                        .Validate(config => !string.IsNullOrWhiteSpace(config.SecretKey), "SecretKey is required")
                        .ValidateDataAnnotations();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = jwtSettings.Issuer,
                   ValidAudience = jwtSettings.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
               };
           });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(opt =>
        {
            //Just during development!
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 3;

            opt.User.RequireUniqueEmail = true;
        })
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
    }
}

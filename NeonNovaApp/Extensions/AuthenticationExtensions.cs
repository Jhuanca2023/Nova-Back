using System.Text;
using Domain.Entities;
using Intrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace NeonNovaApp.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        var KeyJwt = Environment.GetEnvironmentVariable("key_jwt");
        var GOOGLE_CLIENT_ID = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        var GOOGLE_SECRET_CLIENT = Environment.GetEnvironmentVariable("GOOGLE_SECRET_CLIENT");
        services.AddIdentityCore<Users>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequiredLength = 6;
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<UserManager<Users>>();
        services.AddScoped<SignInManager<Users>>();
        services.AddHttpContextAccessor();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie().AddGoogle(opt =>
        {
            if (GOOGLE_SECRET_CLIENT is null)
            {
                throw new ArgumentException(nameof(GOOGLE_SECRET_CLIENT));
            }

            if (GOOGLE_CLIENT_ID is null)
            {
                throw new ArgumentException(nameof(GOOGLE_CLIENT_ID));
            }

            opt.ClientId = GOOGLE_CLIENT_ID;
            opt.ClientSecret = GOOGLE_SECRET_CLIENT;
            opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.Scope.Add("profile");
            opt.Scope.Add("email");
            opt.ClaimActions.MapJsonKey("picture", "picture");
            opt.SaveTokens = true;
        }).AddJwtBearer(opt =>
        {
            opt.MapInboundClaims = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyJwt!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("isAdmin", politica => politica.RequireClaim("isAdmin"));
            opt.AddPolicy("isUser", politica => politica.RequireClaim("isUser"));
        });

        return services;
    }
}
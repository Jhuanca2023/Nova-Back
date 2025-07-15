using System.Text;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using DotNetEnv;
using Intrastructure.Data;
using Intrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NeonNovaApp.Extensions;
using NeonNovaApp.Middleware;
using NeonNovaApp.Services;
using Microsoft.AspNetCore.HttpOverrides;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opts =>
{
    opts.Limits.MaxRequestBodySize = null;
});
// builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Si lo deseas, limpia KnownNetworks/Proxies para confiar en cualquiera:
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
Env.Load();


builder.Configuration.AddEnvironmentVariables();


// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//   .AddJwtBearer(options =>
//   {
//       options.TokenValidationParameters = new TokenValidationParameters
//       {
//           ValidateIssuer = true,
//           ValidIssuer = builder.Configuration["Jwt:Issuer"],
//
//           ValidateAudience = true,
//           ValidAudience = builder.Configuration["Jwt:Audience"],
//
//           ValidateLifetime = true,
//
//           ValidateIssuerSigningKey = true,
//           IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
//       };
//   });


var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// CORS: permite solicitudes desde Angular
var originsPermits = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins(originsPermits)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


builder.Services.AddAuthenticationConfiguration();


builder.Services.AddHttpServices();
builder.Services.AddApplicationServices();
builder.Services.AddMappingConfiguration();

builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<ICartShopService, CartShopService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();


builder.Services.AddDocumentationServices();


builder.Services.AddMonitoringServices();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseForwardedHeaders();
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
// });
// app.Use(async (context, next) =>
// {
//     context.Request.EnableBuffering();  
//     await next();
// });

await DataSeeder.SeedUsers(app.Services);
// Middlewares


app.UseGlobalExceptionHandler();


// app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/checkout/webhook"),
//     appBuilder => appBuilder.UseHttpsRedirection());
if (app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}


app.UseRouting();
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();


app.UseDocumentationMiddleware();

app.UseMonitoringMiddleware();


app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();
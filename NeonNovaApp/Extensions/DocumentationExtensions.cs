using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace NeonNovaApp.Extensions;

public static class DocumentationExtensions
{
    public static IServiceCollection AddDocumentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApi(options =>
        {
            /* Configuración de OpenAPI */
        });

        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static void UseDocumentationMiddleware(this WebApplication app)
    {
        app.UseSwagger(opt => { opt.RouteTemplate = "openapi/{documentName}.json"; });

     
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/v1.json", "NeonNovaAPI v1");
                c.RoutePrefix = "swagger";
            });

            // Documentación con Scalar
            app.MapScalarApiReference(opt =>
            {
                opt
                    .WithTitle("NeonNovaApi")
                    .WithTheme(ScalarTheme.Mars)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .WithPreferredScheme("Api Scheme")
                    .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11)
                    .WithTheme(ScalarTheme.BluePlanet);
            });
        
    }
}
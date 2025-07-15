namespace NeonNovaApp.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        // Cliente HTTP general
        services.AddHttpClient();
        
        return services;
    }
}
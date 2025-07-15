using Prometheus;

namespace NeonNovaApp.Extensions;

public static class MonitoringExtensions
{
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
    {
        services.AddHealthChecks().ForwardToPrometheus();
        return services;
    }

    public static void UseMonitoringMiddleware(this WebApplication app)
    {
        // app.UseRouting();
        app.UseHttpMetrics();
        app.UseMetricServer();
        app.MapMetrics();
        app.MapHealthChecks("/health");
        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapControllers();
        //     endpoints.MapMetrics();
        //     endpoints.MapHealthChecks("/health");
        // });
    }
}
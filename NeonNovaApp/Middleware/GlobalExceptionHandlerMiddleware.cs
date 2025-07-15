using System.Net;
using System.Text.Json;

namespace NeonNovaApp.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware (
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = 404,
                        message = "El recurso solicitado no existe",
                        detail = $"La ruta '{context.Request.Path}' no fue encontrada"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }

                if (context.Response.StatusCode == 405 && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = 405,
                        message = "Método HTTP no permitido",
                        detail = $"El método '{context.Request.Method}' no está permitido para esta ruta"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync (HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Ha ocurrido un error inesperado.";
            var detail = _env.IsDevelopment() ? exception.StackTrace : null;

            if (exception is ArgumentNullException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Falta un parámetro requerido.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                message = "El recurso solicitado no fue encontrado.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "No tienes permiso para acceder a este recurso.";
            }

            var response = new
            {
                status = (int)statusCode,
                message = message,
                detail = detail
            };

            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler (
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}

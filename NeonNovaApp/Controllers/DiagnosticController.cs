using Microsoft.AspNetCore.Mvc;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/diagnostic")]
    public class DiagnosticController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DiagnosticController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.UtcNow });
        }

        [HttpGet("https")]
        public IActionResult CheckHttps()
        {
            return Ok(new
            {
                isHttps = Request.IsHttps,
                scheme = Request.Scheme,
                host = Request.Host.Value,
                pathBase = Request.PathBase.Value,
                path = Request.Path.Value
            });
        }

        [HttpGet("webhook-config")]
        public IActionResult WebhookConfig()
        {
            var stripeWebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
            var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

            return Ok(new
            {
                webhookSecretConfigured = !string.IsNullOrEmpty(stripeWebhookSecret),
                stripeKeyConfigured = !string.IsNullOrEmpty(stripeSecretKey),
                forwardedProto = Request.Headers["X-Forwarded-Proto"].FirstOrDefault(),
                forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault(),
                host = Request.Host.Value,
                isHttps = Request.IsHttps,
                scheme = Request.Scheme
            });
        }

        [HttpPost("webhook-test")]
        public async Task<IActionResult> TestWebhook()
        {
            var requestData = new
            {
                timestamp = DateTime.UtcNow,
                headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                isHttps = Request.IsHttps,
                scheme = Request.Scheme
            };

            // Leer el cuerpo si existe
            string body = "No body";
            if (Request.ContentLength.HasValue && Request.ContentLength > 0)
            {
                Request.EnableBuffering();
                Request.Body.Position = 0;
                body = await new StreamReader(Request.Body).ReadToEndAsync();
                Request.Body.Position = 0;
            }

            return Ok(new
            {
                message = "Webhook test received successfully",
                requestData,
                body
            });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy" });
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                bool canConnect = await _dbContext.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new
                    {
                        message = "Conexión a la base de datos exitosa",
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "No se pudo conectar a la base de datos",
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al conectar con la base de datos",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
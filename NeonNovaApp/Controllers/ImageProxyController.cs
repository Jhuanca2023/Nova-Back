using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeonNovaApp.Controllers;

[Route("api/proxy")]
[ApiController]
public class ImageProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ImageProxyController> _logger;
    private readonly IWebHostEnvironment _environment;

    public ImageProxyController(IHttpClientFactory httpClientFactory, ILogger<ImageProxyController> logger, IWebHostEnvironment environment)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet("image")]
    [AllowAnonymous]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> ProxyImage([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL de imagen no proporcionada");
        }

        try
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException ex)
            {
                _logger.LogWarning(ex, "URL de imagen inválida: {Url}", url);
                return BadRequest("URL de imagen inválida");
            }

            string host = uri.Host.ToLower();

            // Para imágenes de Google, SIEMPRE redirigir directamente
            if (host.Contains("googleusercontent.com") || host.Contains("ggpht.com"))
            {
                _logger.LogInformation("Redirigiendo directamente a imagen de Google: {Url}", url);
                return Redirect(url);
            }

            string[] allowedDomains = new[] {
                "res.cloudinary.com",
                "cloudinary.com",
                "neonnova.netlify.app",
                "localhost"
            };

            bool isDomainAllowed = allowedDomains.Any(domain => host.EndsWith(domain));

            if (!isDomainAllowed)
            {
                _logger.LogWarning("Dominio no permitido: {Host}", host);
                return BadRequest("Dominio de imagen no permitido");
            }

            // Para otras imágenes (no de Google), usar el proxy
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error al obtener imagen: {StatusCode} - {Url}", response.StatusCode, url);
                return ReturnDefaultAvatar();
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            if (imageBytes == null || imageBytes.Length == 0)
            {
                _logger.LogWarning("Imagen vacía recibida de: {Url}", url);
                return ReturnDefaultAvatar();
            }

            Response.Headers.Add("Cache-Control", "public, max-age=86400");
            Response.Headers.Add("Pragma", "cache");
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(1).ToString("R"));

            return File(imageBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar imagen de {Url}: {Message}", url, ex.Message);
            return ReturnDefaultAvatar();
        }
    }

    private IActionResult ReturnDefaultAvatar()
    {
        try
        {
            // Intenta primero servir el archivo existente
            string defaultAvatarPath = Path.Combine(_environment.WebRootPath, "default-avatar.png");

            if (System.IO.File.Exists(defaultAvatarPath))
            {
                return PhysicalFile(defaultAvatarPath, "image/png");
            }

            var svgContent = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""200"" height=""200"" viewBox=""0 0 200 200"">
                <rect width=""200"" height=""200"" fill=""#808080""/>
                <text x=""50%"" y=""50%"" dominant-baseline=""middle"" text-anchor=""middle"" font-family=""Arial"" font-size=""80"" fill=""#FFFFFF"">?</text>
            </svg>";

            return Content(svgContent, "image/svg+xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al devolver avatar predeterminado");
            return Content("Error al cargar imagen", "text/plain");
        }
    }
}
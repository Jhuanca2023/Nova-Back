using Application.DTOs.CheckoutDTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger<CheckoutController> _logger;
    private readonly ICartShopService _cartShopService; // Se añade la interfaz ICartShopService

    public CheckoutController(
        ICheckoutService checkoutService,
        ILogger<CheckoutController> logger,
        ICartShopService cartShopService) // Se añade el servicio
    {
        _checkoutService = checkoutService;
        _logger = logger;
        _cartShopService = cartShopService; // Se asigna el servicio
    }

    [HttpPost("personal-info")]
    public async Task<IActionResult> PersonalInfo([FromBody] PersonalInfoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            _logger.LogInformation("📦 Dirección recibida: {Street}, {City}, {Postal}",
                dto.Address.Street, dto.Address.City, dto.Address.PostalCode);

            var addressId = await _checkoutService.SavePersonalInfoAsync(dto);

            return Ok(new { message = "Dirección de envío guardada correctamente", addressId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar información personal");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("payment-method")]
    public async Task<IActionResult> SetPaymentMethod([FromBody] PaymentMethodDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var paymentMethodId = await _checkoutService.CreatePaymentMethodAsync(dto);
            return Ok(new { paymentMethodId });
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear método de pago");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequestDto req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            // Definir las URLs de éxito y cancelación
            req.SuccessUrl = "https://neonnova.netlify.app/success?session_id={CHECKOUT_SESSION_ID}";
            req.CancelUrl = "https://neonnova.netlify.app/cancel";


            var url = await _checkoutService.CreateCheckoutSessionAsync(req);
            return Ok(new { url });
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERROR EN CREAR SESIÓN STRIPE");
            return StatusCode(500, "Error al crear la sesión de pago.");
        }
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook()
    {
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        Request.Body.Position = 0;  

        try
        {
            _logger.LogInformation("Webhook received");

            // Use X-Forwarded-Proto header to determine if the original request was HTTPS
            // Azure Container Apps agrega este encabezado cuando termina SSL
            var forwardedProto = Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            var isSecure = forwardedProto == "https" || Request.IsHttps;

            _logger.LogInformation(
                $"Esquema: {Request.Scheme}, Forwarded-Proto: {forwardedProto}, Es seguro: {isSecure}");

            if (!isSecure)
            {
                _logger.LogWarning("Webhook recibido a través de HTTP inseguro - Stripe requiere HTTPS");
                // Stripe requiere HTTPS, pero si estamos detrás de un proxy/load balancer
                // que termina SSL, podemos continuar ya que la conexión externa es segura
            }

            var signature = Request.Headers["Stripe-Signature"];
            _logger.LogInformation($"Signature recibida: {(string.IsNullOrEmpty(signature) ? "No" : "Sí")}");

            var success = await _checkoutService.ProcessWebhookAsync(json, signature);

            if (success)
            {
                var user = await _checkoutService.GetCurrentUserAsync();
                var userId = user?.Id;

                if (!string.IsNullOrEmpty(userId))
                {
                    await _cartShopService.ClearCartAsync(userId);
                }
            }

            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error en webhook Stripe");
            return BadRequest();
        }
    }

    [HttpGet("checkout/session/{id}")]
    public async Task<IActionResult> GetSessionDetails(string id)
    {
        var session = await _checkoutService.GetSessionDetailsAsync(id);
        return Ok(session);
    }

    [HttpPost("cart/save")]
    public async Task<IActionResult> SaveCart([FromBody] SaveCartDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _checkoutService.SaveCartAsync(dto);
            return Ok();
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el carrito");
            return StatusCode(500, "Error al guardar el carrito.");
        }
    }
}
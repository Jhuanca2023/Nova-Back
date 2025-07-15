using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SignInManager<Users> _signInManager;
    private readonly LinkGenerator _linkGenerator;

    public AuthController(IAuthService authService, SignInManager<Users> signInManager, LinkGenerator linkGenerator)
    {
        _authService = authService;
        _signInManager = signInManager;
        _linkGenerator = linkGenerator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponseDto>> Register(CredentialsUserDto dto)
    {
        try
        {
            var response = await _authService.Register(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponseDto>> Login(LoginRequestDto dto)
    {
        try
        {
            var response = await _authService.Login(dto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("removate-token")]
    public async Task<ActionResult<AuthenticationResponseDto>> RenovateToken()
    {
        try
        {
            var response = await _authService.RenovateToken();
            return Ok(response);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
    }


    [HttpPut("admin-status")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> UpdateAdminStatus([FromBody] AdminStatusUpdateDto dto)
    {
        try
        {
            await _authService.UpdateAdminStatusAsync(dto.userId, dto.IsAdmin);
            return NoContent();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("login/google")]
    public IActionResult LoginGoogle([FromQuery] string returnUrl = "/")
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google",
            _linkGenerator.GetPathByName("LoginGoogleCallback") + $"?returnUrl={returnUrl}");

        return Challenge(properties, "Google");
    }

    [HttpGet("login/google/callback", Name = "LoginGoogleCallback")]
    public async Task<IActionResult> LoginGoogleCallback([FromQuery] string returnUrl = "/")
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        try
        {
            var authResponse = await _authService.LoginWithGoogleAsync(result.Principal);

            if (returnUrl.Contains("token") || string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
            {
                return Ok(authResponse);
            }

            if (returnUrl.StartsWith("http://localhost:4200") ||
                returnUrl.StartsWith("https://neonnova.netlify.app"))
            {
                return Redirect($"{returnUrl}?token={authResponse.Token}");
            }

            return Redirect(returnUrl);
        }
        catch (UnauthorizedAccessException ex)
        {
            if (returnUrl.StartsWith("http://localhost:4200") ||
                returnUrl.StartsWith("https://neonnova.netlify.app"))
            {
                return Redirect($"{returnUrl}?error={Uri.EscapeDataString(ex.Message)}");
            }

            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
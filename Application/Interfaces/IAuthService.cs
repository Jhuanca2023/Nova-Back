using System.Security.Claims;
using Application.DTOs.AuthDTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthenticationResponseDto> Register(CredentialsUserDto dto);
    Task<AuthenticationResponseDto> Login(LoginRequestDto dto);
    Task<AuthenticationResponseDto> RenovateToken();
    Task<AuthenticationResponseDto> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal);
    Task UpdateAdminStatusAsync(string userId, bool isAdmin);
}
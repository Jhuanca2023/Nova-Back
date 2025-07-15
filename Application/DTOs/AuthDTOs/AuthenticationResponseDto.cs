namespace Application.DTOs.AuthDTOs;

public class AuthenticationResponseDto
{
    public required string Token { get; set; }
    public DateTime Expired { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthDTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string? Password { get; set; }
}
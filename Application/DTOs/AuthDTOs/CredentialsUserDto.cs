using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthDTOs;

public class CredentialsUserDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El nombre no debe superar los 500 caracteres.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El apellido no debe superar los 500 caracteres.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El correo electrónico no debe superar los 500 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string? Password { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthDTOs;

public class EditClaimDto
{
    [EmailAddress] [Required] public required string Email { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CategoryDTOs;

public class UpdateCategoryRequestDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El nombre de la categoría no debe superar los 500 caracteres.")]
    public string Name { get; set; }

    [MaxLength(500, ErrorMessage = "La descripción de la categoría no debe superar los 500 caracteres.")]
    public string Description { get; set; }
}
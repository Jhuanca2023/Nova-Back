using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Favorite;

public class AddFavoriteDto
{
    [Required(ErrorMessage = "El ID del producto es obligatorio")]
    public int ProductId { get; set; }
}
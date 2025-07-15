using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CartShopDTOs;

public class UpdateCartShopItemDto
{
    public int CartDetailId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Quantity { get; set; }
}
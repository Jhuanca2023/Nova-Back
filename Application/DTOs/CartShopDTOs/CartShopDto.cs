namespace Application.DTOs.CartShopDTOs;

public class CartShopDto
{
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }
    public string Status { get; set; }
    public ICollection<CartShopDetailDto> Details { get; set; } = new List<CartShopDetailDto>();
    public decimal Total => Details.Sum(d => d.Subtotal);
}
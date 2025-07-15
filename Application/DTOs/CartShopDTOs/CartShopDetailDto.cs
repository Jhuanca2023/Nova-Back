namespace Application.DTOs.CartShopDTOs;

public class CartShopDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}
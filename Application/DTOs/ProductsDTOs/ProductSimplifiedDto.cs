using Domain.Enums;

namespace Application.DTOs.ProductsDTOs;

public class ProductoSimplificadoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public ProductStatus Status { get; set; }
    public int Stock { get; set; }
    public int? Punctuation { get; set; } = 0;
    public string ImageUrl { get; set; }
}
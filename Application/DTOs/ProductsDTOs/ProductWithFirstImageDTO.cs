using Application.DTOs.CategoryDTOs;
using Domain.Enums;

namespace Application.DTOs.ProductsDTOs;

public class ProductWithFirstImageDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public CategoryDto Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Stock { get; set; }
    public int? Punctuation { get; set; }
    public ProductStatus Status { get; set; }
    public ProductImageDTO FirstImage { get; set; }
}
using Domain.Enums;

namespace Domain.Entities;

public class ProductSimplified
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int? Punctuation { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; }
    public string ImageUrl { get; set; }
}
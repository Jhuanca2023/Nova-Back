using Domain.Enums;

namespace Application.DTOs.ProductsDTOs;

public class ProductImageDTO
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
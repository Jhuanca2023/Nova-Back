using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ProductsDTOs;

public class UpdateProductImageDTO
{
    public int ImageId { get; set; }
    public IFormFile Image { get; set; }
}
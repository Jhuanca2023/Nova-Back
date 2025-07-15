using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ProductsDTOs
{
    public record UpdateProductRequestDTO(
        IFormFile? Image,
        string? Name,
        decimal? Price,
        string? Description,
        int? Stock,
        int? CategoryId,
        ProductStatus? Status
    );
}
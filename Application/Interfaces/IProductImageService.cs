using Application.DTOs.ProductsDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IProductImageService
{
    Task<ProductImageDTO> AddImageAsync(int productId, IFormFile image);
    Task DeleteImageAsync(int imageId);
    Task<List<ProductImageDTO>> GetImagesProductIdAsync(int productId);
    Task<ProductImage> GetPrimaryImageAsync(int productId);
}
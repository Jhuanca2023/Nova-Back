using Application.DTOs.Common;
using Application.DTOs.ProductsDTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDTO>> GetAllAsync();
    Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO dto);

    Task<ProductPaginatedResponseDto> GetAllPaginatedAsync(
        int pageNumber, int pageSize, ProductStatus? status = null
    );

    Task<ProductWithCommentsPaginatedResponseDto> GetAllPaginatedWithCommentsAsync(
        int pageNumber, int pageSize, ProductStatus? status = null
    );
    Task<int> UpdateOutOfStockProductsAsync();
    Task<ProductResponseDTO> GetByIdWithImagesAsync(int id);
    Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO dto);
    Task DeleteAsync(int id);

    Task<ProductImageDTO> UpdateImageAsync(int productId, int imageId, IFormFile image);
    Task<PaginatedResponseDto<ProductoSimplificadoDto>> GetProductSimplified(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        string searchTerm = null
    );
    Task<ProductWithPaginatedCommentsDto> GetProductWithPaginatedCommentsAsync(
        int productId, int commentsPage, int commentsPageSize);

    Task<PaginatedResponseDto<ProductoSimplificadoDto>> GetProductsFormAdmin(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        ProductStatus? status = default,
        string searchTerm = null
    );
}
using Application.DTOs.CategoryDTOs;
using Application.DTOs.Common;
using Application.DTOs.ProductsDTOs;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<PaginatedResponseDto<CategoryWithProductCountDto>> GetAllPaginatedAsync(int pageNumber, int pageSize);
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> AddCategoryAsync(CreateCategoryRequestDto category);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto category);
    Task<List<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId);

    Task<PaginatedResponseDto<ProductWithFirstImageDTO>> GetProductsByCategoryWithFirstImagePaginatedAsync(
        int categoryId, int pageSize, int pageNumber);

    Task DeleteCategoryAsync(int id);
}
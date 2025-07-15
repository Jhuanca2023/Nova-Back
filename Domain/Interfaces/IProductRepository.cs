using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<PagedResult<Product>> GetAllPaginatedAsync(int pageNumber, int pageSize, ProductStatus? status = null);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeletedAsync(int id);
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetAllWithCategoriesAsync();
        Task<PagedResult<Product>> GetProductsByCategoryPaginatedAsync(int categoryId, int pageNumber, int pageSize);
        Task<IEnumerable<ProductSimplified>> GetAllProductSimplifiedAsync();
        Task UpdateProductPunctuationAsync(int productId);
        Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);
        Task<int> UpdateOutOfStockProductsAsync();
        Task<PagedResult<ProductSimplified>> GetProductsForAdminAsync(
            int pageNumber,
            int pageSize,
            int? categoryId = null,
            ProductStatus? status = null, string searchTerm = null);

        Task<PagedResult<ProductSimplified>> GetAllProductSimplifiedPaginatedAsync(
            int pageNumber,
            int pageSize,
            int? categoryId = null,
            string searchTerm = null
        );
    }
}
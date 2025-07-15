using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductImageRepository
{
    Task<ProductImage> GetByIdAsync(int id);
    Task AddAsync(ProductImage image);
    Task DeleteAsync(ProductImage image);
    Task<List<ProductImage>> GetAllByProductIdAsync(int productId);
    IQueryable<ProductImage> GetAllByProductId(int productId);
    Task UpdateAsync(ProductImage image);
}
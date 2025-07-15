using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductCommentRepository
{
    Task<ProductComment> AddAsync(ProductComment comment);
    Task<ProductComment> UpdateAsync(ProductComment comment);
    Task<ProductComment> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByProductAndUserAsync(int productId, string userId);
    Task<ProductComment?> GetByProductAndUserAsync(int productId, string userId);
    Task<List<ProductComment>> GetCommentsByProductIdAsync(int productId);
    Task<PagedResult<ProductComment>> GetPaginatedCommentsByProductIdAsync(int productId, int pageNumber, int pageSize);
    
}
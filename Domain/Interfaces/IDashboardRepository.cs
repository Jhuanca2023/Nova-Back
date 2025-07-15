using Domain.Entities;

namespace Domain.Interfaces;

public interface IDashboardRepository
{
    // Estadísticas de usuarios
    Task<int> GetTotalUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
    Task<int> GetNewUsersCountInPeriodAsync(DateTime startDate, DateTime endDate);

    // Estadísticas de productos
    Task<int> GetTotalProductsCountAsync();
    Task<int> GetNewProductsCountInPeriodAsync(DateTime startDate, DateTime endDate);
    Task<List<Product>> GetLowStockProductsAsync(int threshold = 10);

    // Estadísticas de categorías
    Task<int> GetTotalCategoriesCountAsync();
    Task<int> GetNewCategoriesCountInPeriodAsync(DateTime startDate, DateTime endDate);
    Task<List<Category>> GetCategoriesWithProductCountAsync(int limit = 5);
}
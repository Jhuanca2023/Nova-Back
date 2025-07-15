using Application.DTOs.DashboardDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;

namespace Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;
    private readonly IMapper _mapper;

    public DashboardService(IDashboardRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            UserStats = await GetUserStatsAsync(),
            ProductStats = await GetProductStatsAsync(),
            CategoryStats = await GetCategoryStatsAsync()
        };
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var totalUsers = await _repository.GetTotalUsersCountAsync();
        var activeUsers = await _repository.GetActiveUsersCountAsync();

        var weekAgo = DateTime.UtcNow.AddDays(-7);
        var newUsersThisWeek = await _repository.GetNewUsersCountInPeriodAsync(weekAgo, DateTime.UtcNow);

        var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
        var newUsersPreviousWeek = await _repository.GetNewUsersCountInPeriodAsync(twoWeeksAgo, weekAgo);

        double activePercentage = totalUsers > 0 ? (double)activeUsers / totalUsers * 100 : 0;
        double growthPercentage = newUsersPreviousWeek > 0
            ? ((double)newUsersThisWeek - newUsersPreviousWeek) / newUsersPreviousWeek * 100
            : (newUsersThisWeek > 0 ? 100 : 0);

        return new UserStatsDto
        {
            ActiveUsersCount = activeUsers,
            ActiveUsersPercentage = Math.Round(activePercentage, 1),
            TotalUsersCount = totalUsers,
            NewUsersThisWeek = newUsersThisWeek,
            NewUsersPercentage = Math.Round(growthPercentage, 1)
        };
    }

    public async Task<ProductStatsDto> GetProductStatsAsync()
    {
        var totalProducts = await _repository.GetTotalProductsCountAsync();

        var monthAgo = DateTime.UtcNow.AddMonths(-1);
        var newProductsLastMonth = await _repository.GetNewProductsCountInPeriodAsync(monthAgo, DateTime.UtcNow);

        var twoMonthsAgo = DateTime.UtcNow.AddMonths(-2);
        var newProductsPreviousMonth = await _repository.GetNewProductsCountInPeriodAsync(twoMonthsAgo, monthAgo);

        double growthPercentage = newProductsPreviousMonth > 0
            ? ((double)newProductsLastMonth - newProductsPreviousMonth) / newProductsPreviousMonth * 100
            : (newProductsLastMonth > 0 ? 100 : 0);

        var lowStockProducts = await _repository.GetLowStockProductsAsync();

        return new ProductStatsDto
        {
            TotalProductsCount = totalProducts,
            ProductsGrowthPercentage = Math.Round(growthPercentage, 1),
            LowStockProductsCount = lowStockProducts.Count,
            LowStockProducts = lowStockProducts.Select(p => new LowStockProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock
            }).ToList()
        };
    }

    public async Task<CategoryStatsDto> GetCategoryStatsAsync()
    {
        var totalCategories = await _repository.GetTotalCategoriesCountAsync();

        var monthAgo = DateTime.UtcNow.AddMonths(-1);
        var newCategoriesLastMonth = await _repository.GetNewCategoriesCountInPeriodAsync(monthAgo, DateTime.UtcNow);

        var twoMonthsAgo = DateTime.UtcNow.AddMonths(-2);
        var newCategoriesPreviousMonth = await _repository.GetNewCategoriesCountInPeriodAsync(twoMonthsAgo, monthAgo);

        double growthPercentage = newCategoriesPreviousMonth > 0
            ? ((double)newCategoriesLastMonth - newCategoriesPreviousMonth) / newCategoriesPreviousMonth * 100
            : (newCategoriesLastMonth > 0 ? 100 : 0);

        var topCategories = await _repository.GetCategoriesWithProductCountAsync();

        return new CategoryStatsDto
        {
            TotalCategoriesCount = totalCategories,
            CategoriesGrowthPercentage = Math.Round(growthPercentage, 1),
            ActiveCategoriesCount = topCategories.Count,
            TopCategories = topCategories.Select(c => new CategoryUsageDto
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count
            }).ToList()
        };
    }
}
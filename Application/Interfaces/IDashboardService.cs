using Application.DTOs.DashboardDTOs;

namespace Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<UserStatsDto> GetUserStatsAsync();
    Task<ProductStatsDto> GetProductStatsAsync();
    Task<CategoryStatsDto> GetCategoryStatsAsync();
}
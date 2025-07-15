namespace Application.DTOs.DashboardDTOs;

public class CategoryStatsDto
{
    public int TotalCategoriesCount { get; set; }
    public double CategoriesGrowthPercentage { get; set; }
    public int ActiveCategoriesCount { get; set; }
    public List<CategoryUsageDto> TopCategories { get; set; }
}
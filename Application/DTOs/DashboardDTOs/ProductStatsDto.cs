namespace Application.DTOs.DashboardDTOs;

public class ProductStatsDto
{
    public int TotalProductsCount { get; set; }
    public double ProductsGrowthPercentage { get; set; }
    public int LowStockProductsCount { get; set; }
    public List<LowStockProductDto> LowStockProducts { get; set; }
}
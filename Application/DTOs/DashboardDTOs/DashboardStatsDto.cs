namespace Application.DTOs.DashboardDTOs;

public class DashboardStatsDto
{
    public UserStatsDto UserStats { get; set; }
    public ProductStatsDto ProductStats { get; set; }
    public CategoryStatsDto CategoryStats { get; set; }
}
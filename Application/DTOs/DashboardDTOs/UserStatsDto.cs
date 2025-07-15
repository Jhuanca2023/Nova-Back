namespace Application.DTOs.DashboardDTOs;

public class UserStatsDto
{
    public int ActiveUsersCount { get; set; }
    public double ActiveUsersPercentage { get; set; }
    public int TotalUsersCount { get; set; }
    public int NewUsersThisWeek { get; set; }
    public double NewUsersPercentage { get; set; }
}
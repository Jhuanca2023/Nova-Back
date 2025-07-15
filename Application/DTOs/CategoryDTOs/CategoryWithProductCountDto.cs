namespace Application.DTOs.CategoryDTOs;

public class CategoryWithProductCountDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductCount { get; set; }
}
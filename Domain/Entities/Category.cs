namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Una categoría puede tener muchos productos.
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
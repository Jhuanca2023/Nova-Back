using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Favorite
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Propiedades de navegación
    public Users User { get; set; }
    public Product Product { get; set; }
}
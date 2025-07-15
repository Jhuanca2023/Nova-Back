using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Users : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    // Colecciones de navegación.
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ProductComment> Comments { get; set; } = new List<ProductComment>();
    public ICollection<CartShop> Carts { get; set; } = new List<CartShop>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }

    [Required] public string UserId { get; set; }

    public DateTime Date { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public OrderStatus Status { get; set; }


    // Propiedad de navegación: cada orden pertenece a un usuario.
    public Users Users { get; set; }

    // 🔽 Nueva relación con la dirección
    public int? ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; }

    // Colecciones de navegación.
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    
}
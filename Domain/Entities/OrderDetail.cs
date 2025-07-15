using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class OrderDetail
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La orden es obligatoria.")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Propiedades de navegación.
    public Order Order { get; set; }
    public Product Product { get; set; }
}
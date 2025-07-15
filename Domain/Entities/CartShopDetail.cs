using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class CartShopDetail
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El carrito es obligatorio.")]
    public int CartId { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Propiedades de navegación.
    public CartShop CartShop { get; set; }
    public Product Product { get; set; }
}
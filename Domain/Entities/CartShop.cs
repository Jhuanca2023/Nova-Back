using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class CartShop
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio para el carrito.")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
    public DateTime CreationDate { get; set; }

    [Required(ErrorMessage = "El estado del carrito es obligatorio.")]
    public CartShopStatus Status { get; set; }

    // Propiedades de navegación.
    [ForeignKey("UserId")] public Users User { get; set; }
    public ICollection<CartShopDetail> CartShopDetails { get; set; } = new List<CartShopDetail>();
}
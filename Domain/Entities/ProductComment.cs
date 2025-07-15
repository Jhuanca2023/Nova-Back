using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class ProductComment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio para el comentario.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio para el comentario.")]
    public string  UserId { get; set; }

    [Required(ErrorMessage = "El comentario es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El comentario no debe superar los 500 caracteres.")]
    public string Comment { get; set; }

    [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Date { get; set; }

    // Propiedades de navegación.
    public Product Product { get; set; }
    
    [ForeignKey("UserId")]
    public Users User { get; set; }
}
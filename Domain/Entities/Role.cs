using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Role
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El nombre del rol no debe superar los 500 caracteres.")]
    public string Name { get; set; }

    [MaxLength(500, ErrorMessage = "La descripción del rol no debe superar los 500 caracteres.")]
    public string Description { get; set; }

    // Un rol puede tener muchos usuarios.
    public ICollection<Users> Users { get; set; } = new List<Users>();
}
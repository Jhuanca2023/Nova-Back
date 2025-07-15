using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ComentDTOs
{
    public class CreateCommentDto
    {
        
        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [MaxLength(500, ErrorMessage = "El comentario no debe superar los 500 caracteres.")]
        public string Comment { get; set; }

        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Rating { get; set; }
        
    }
}

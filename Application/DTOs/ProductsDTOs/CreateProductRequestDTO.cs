using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ProductsDTOs
{
    public record CreateProductRequestDTO
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(500, ErrorMessage = "El nombre del producto no debe superar los 500 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción del producto no debe superar los 500 caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio del producto es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock del producto es obligatorio.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría del producto es obligatoria.")]
        public int CategoryId { get; set; }

        public ProductStatus Status { get; set; }
        
        public List<IFormFile>? Images { get; set; }
    }
}
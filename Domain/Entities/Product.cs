using Domain.Enums;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public int CategoryId { get; set; }
        
        public int? Punctuation { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProductStatus Status { get; set; }

        // Propiedad de navegación: cada producto pertenece a una categoría.
        public Category Category { get; set; }

        // Propiedades de navegación.
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductComment> Comments { get; set; } = new List<ProductComment>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<CartShopDetail> CartShopDetails { get; set; } = new List<CartShopDetail>();
    }
}
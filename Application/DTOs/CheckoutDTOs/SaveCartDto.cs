using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class SaveCartDto
    {
       
        public List<CartItemDto> Items { get; set; }

        public ShippingAddressDto? ShippingAddress { get; set; }
    }
    public class ShippingAddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // ← ESTA ES LA QUE FALTABA
    }

}

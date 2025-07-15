using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class CheckoutCompleteDto
    {
        public int UserId { get; set; }
        public int CartId { get; set; }
        public string PaymentMethodId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public decimal ShippingCost { get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public AddressDto ShippingAddress { get; set; } 

    }
}

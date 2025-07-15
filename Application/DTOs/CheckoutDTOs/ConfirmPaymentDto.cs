using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class ConfirmPaymentDto
    {

        public int UserId { get; set; }
        public int CartId { get; set; }
        public string PaymentMethodId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        public decimal ShippingCost { get; set; }
    }
}

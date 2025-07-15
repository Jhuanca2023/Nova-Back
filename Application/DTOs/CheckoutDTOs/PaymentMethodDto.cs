using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class PaymentMethodDto
    {
     
        public string PaymentMethod { get; set; }
 
        public CardDto Card { get; set; }
    }

    public class CardDto
    {
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }  
        public int ExpYear { get; set; }  
        public string CVV { get; set; }
    }
}

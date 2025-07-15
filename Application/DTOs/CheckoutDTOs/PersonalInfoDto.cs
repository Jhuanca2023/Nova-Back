using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class PersonalInfoDto
    {
        public string FirstName { get; set; }    
        public string LastName { get; set; }      
        public string Email { get; set; }
        public string Phone { get; set; }
 
       public AddressDto Address { get; set; }
        
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
    }

    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class ConfirmOrderDto
    {
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }



}

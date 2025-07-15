using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OrderDTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public List<OrderDetailDto> Details { get; set; }
    }

    public class OrderDetailDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}


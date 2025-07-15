using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CheckoutDTOs
{
    public class CheckoutSessionRequestDto
    {
        public List<LineItemDto> LineItems { get; set; }
        public string Currency { get; set; }

        public long ShippingCost { get; set; }    
        public string CustomerEmail { get; set; } 

        public string CustomerPhone { get; set; }

        // Nuevas propiedades para las URLs de redirección
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    public class LineItemDto
    {
        public PriceDataDto PriceData { get; set; }
        public long Quantity { get; set; } // ← NO es nullable
    }

    public class PriceDataDto
    {
        public string Currency { get; set; }
        public ProductDataDto ProductData { get; set; }
        public long UnitAmount { get; set; }
    }

    public class ProductDataDto
    {
        public string Name { get; set; }

        public Dictionary<string, string> Metadata { get; set; } = new();

    }
}
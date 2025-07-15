namespace Domain.Entities;

// Tabla para direcciones - Opcional 
public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public bool IsPrimary { get; set; }
    public Users Users { get; set; }

    public decimal ShippingCost { get; set; } // Costo de envío

}
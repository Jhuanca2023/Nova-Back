using Domain.Entities;

namespace Domain.Interfaces;

public interface ICheckoutRepository
{
    Task<Address> SaveShippingAddressAsync(Address address);
    Task<CartShop> GetActiveCartByUserIdAsync(string userId);
    Task<Address> GetLatestShippingAddressByUserIdAsync(string userId);
    Task<bool> ProductExistsAsync(int productId);
    Task<Product> GetProductByIdAsync(int productId);
    Task<Users?> GetUserByEmailAsync(string email);

    Task SaveCartAsync(CartShop cart);
    Task SaveOrderAsync(Order order);
    Task UpdateCartStatusAsync(CartShop cart);
    Task SaveChangesAsync();

    // Nuevo método para limpiar el carrito
    Task ClearCartAsync(string userId);

    //actualizar el stock 
    Task UpdateProductStock(Product product);
}
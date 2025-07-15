using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartShopRepository
{
    Task<CartShop> GetActiveCartByUserIdAsync(string userId);
    Task<CartShop> CreateCartAsync(CartShop cart);
    Task<CartShop> UpdateCartAsync(CartShop cart);
    Task RemoveCartDetailAsync(CartShopDetail detail);
    Task ClearCartDetailsAsync(int cartId);
}

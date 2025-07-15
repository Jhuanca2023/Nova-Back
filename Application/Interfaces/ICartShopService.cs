using Application.DTOs.CartShopDTOs;

namespace Application.Interfaces;

public interface ICartShopService
{
    Task<CartShopDto> GetCurrentCartAsync();
    Task<CartShopDto> AddToCartAsync(AddToCartShopDto dto);
    // Task<CartShopDto> UpdateCartItemAsync(UpdateCartShopItemDto dto);
    Task<CartShopDto> RemoveCartItemAsync(int cartDetailId);

    Task<bool> CompleteCartAsync();
    Task<CartShopDto> RefreshCartAsync();

    Task<bool> ClearCartAsync();
    Task ClearCartAsync(string userId);
    Task<CartShopDto> IncrementCartItemAsync(int cartDetailId);
    Task<CartShopDto> DecrementCartItemAsync(int cartDetailId);
}
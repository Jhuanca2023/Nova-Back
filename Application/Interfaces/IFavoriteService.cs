using Application.DTOs.Favorite;

namespace Application.Interfaces;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteDto>> GetCurrentUserFavoritesAsync();
    Task<FavoriteDto> AddFavoriteAsync(AddFavoriteDto dto);
    Task RemoveFavoriteAsync(int favoriteId);
    Task<bool> ToggleFavoriteAsync(int productId);
    Task<bool> IsProductFavoriteAsync(int productId);
}
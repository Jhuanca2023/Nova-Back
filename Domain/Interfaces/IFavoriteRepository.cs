using Domain.Entities;

namespace Domain.Interfaces;

public interface IFavoriteRepository
{
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);
    Task<Favorite> GetFavoriteByIdAsync(int id);
    Task<Favorite> GetFavoriteByUserAndProductAsync(string userId, int productId);
    Task<Favorite> AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(Favorite favorite);
    Task<bool> FavoriteExistsAsync(string userId, int productId);
}
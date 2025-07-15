using Domain.Entities;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;


public class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Product)
            .ThenInclude(p => p.Category)
            .Include(f => f.Product.Images)
            // Importante: Incluir los comentarios para que pueda calcular la puntuación si es necesario
            .Include(f => f.Product.Comments)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Favorite> GetFavoriteByIdAsync(int id)
    {
        return await _context.Favorites
            .Include(f => f.Product)
            .ThenInclude(p => p.Category)
            .Include(f => f.Product.Images)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Favorite> GetFavoriteByUserAndProductAsync(string userId, int productId)
    {
        return await _context.Favorites
            .Include(f => f.Product)
            .ThenInclude(p => p.Category)
            .Include(f => f.Product.Images)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
    }
    public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task RemoveFavoriteAsync(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> FavoriteExistsAsync(string userId, int productId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
    }
}
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class CartShopRepository : ICartShopRepository
{
    private readonly ApplicationDbContext _context;

    public CartShopRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartShop> GetActiveCartByUserIdAsync(string userId)
    {
        return await _context.CartShops
            .Include(c => c.CartShopDetails)
            .ThenInclude(d => d.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.Status == CartShopStatus.Active);
    }

    public async Task<CartShop> CreateCartAsync(CartShop cart)
    {
        _context.CartShops.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartShop> UpdateCartAsync(CartShop cart)
    {
        _context.Entry(cart).State = EntityState.Modified;
        foreach (var detail in cart.CartShopDetails)
        {
            if (detail.Id > 0)
            {
                _context.Entry(detail).State = EntityState.Modified;
            }
            else
            {
                _context.Entry(detail).State = EntityState.Added;
            }
        }

        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task RemoveCartDetailAsync(CartShopDetail detail)
    {
        _context.CartShopDetails.Remove(detail);
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartDetailsAsync(int cartId)
    {
        var details = await _context.CartShopDetails
            .Where(d => d.CartId == cartId)
            .ToListAsync();
        _context.CartShopDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
    }
}

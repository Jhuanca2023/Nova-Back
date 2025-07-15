using Domain.Entities;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImage> GetByIdAsync(int id)
    {
        return await _context.ProductImages.FindAsync(id);
    }

    public async Task AddAsync(ProductImage image)
    {
        _context.ProductImages.Add(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductImage image)
    {
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductImage>> GetAllByProductIdAsync(int productId)
    {
        return await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync();
    }

    public IQueryable<ProductImage> GetAllByProductId(int productId)
    {
        return _context.ProductImages.Where(pi => pi.ProductId == productId);
    }

    public async Task UpdateAsync(ProductImage image)
    {
        _context.ProductImages.Update(image);
        await _context.SaveChangesAsync();
    }
    
}
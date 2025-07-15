using Domain.Entities;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByIdAsync(int id) => await _context.Categories.FindAsync(id);


    public async Task<List<Category>> GetAllAsync() => await _context.Categories.ToListAsync();

    public async Task<PagedResult<Category>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Categories
            .Include(c => c.Products)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Category>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<List<Product>> GetProductsAsync(int categoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> AddAsync(Category entity)
    {
        await _context.Categories.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Category> UpdateAsync(Category entity)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
using Domain.Entities;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Estadísticas de Usuarios
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return await _context.Users.CountAsync(u => u.LastLogin >= thirtyDaysAgo);
        }

        public async Task<int> GetNewUsersCountInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Users.CountAsync(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate);
        }

        // Estadísticas de Productos
        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<int> GetNewProductsCountInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Products.CountAsync(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
        }

        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.Stock <= threshold && p.Status == Domain.Enums.ProductStatus.Active)
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToListAsync();
        }

        // Estadísticas de Categorías
        public async Task<int> GetTotalCategoriesCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<int> GetNewCategoriesCountInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            // Asumiendo que la entidad Category tiene un campo CreatedAt
            // Si no existe este campo, esta consulta tendría que adaptarse o eliminarse
            return await _context.Categories
                .CountAsync(c => EF.Property<DateTime>(c, "CreatedAt") >= startDate && 
                                 EF.Property<DateTime>(c, "CreatedAt") <= endDate);
        }

        public async Task<List<Category>> GetCategoriesWithProductCountAsync(int limit = 5)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .OrderByDescending(c => c.Products.Count)
                .Take(limit)
                .ToListAsync();
        }
    }
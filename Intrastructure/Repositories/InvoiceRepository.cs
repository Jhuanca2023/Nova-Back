using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Intrastructure.Data;
using Application.Interfaces;

namespace Intrastructure.Repositories;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
        => _context = context;

    public async Task<List<Product>> ObtenerFacturasAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
}
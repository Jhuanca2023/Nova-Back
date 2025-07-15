using Application.Interfaces;
using Domain.Entities;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intrastructure.Repositories
{
    public class InvoiceUserRepository : IInvoiceUserService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceUserRepository(ApplicationDbContext context)
            => _context = context;

        public async Task<List<Users>> ObtenerReporteAsync()
        {
            return await _context.Users
                .ToListAsync();
        }
    }
}
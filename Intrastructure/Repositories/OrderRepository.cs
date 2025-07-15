using Domain.Entities;
using Domain.Interfaces;

using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Añadir orden
        public async Task<Order> Add(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // Obtener todas las órdenes
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Transactions)
                .ToListAsync();
        }

        // Obtener orden por ID
        public async Task<Order> GetById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Transactions)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Obtener órdenes por usuario
        // Obtener órdenes por usuario
        public async Task<IEnumerable<Order>> GetByUser(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .Include(o => o.Transactions)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        // Obtener detalles de una orden
        public async Task<IEnumerable<OrderDetail>> GetOrderDetails(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product)
                .ToListAsync();
        }

        // Obtener transacciones de una orden
        public async Task<IEnumerable<Transaction>> GetTransactions(int orderId)
        {
            return await _context.Transactions
                .Where(t => t.OrderId == orderId)
                .ToListAsync();
        }

        // Eliminar una orden
        public async Task Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        // Actualizar orden
        public async Task<Order> Update(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task IOrderRepository.Update(Order entity)
        {
            return Update(entity);
        }

        public Task<IEnumerable<Order>> GetByUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}

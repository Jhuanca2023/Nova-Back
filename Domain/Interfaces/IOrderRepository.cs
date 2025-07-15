using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetById(int id);
    Task<IEnumerable<Order>> GetAll();
    Task<IEnumerable<Order>> GetByUser(int userId);
    Task<IEnumerable<Order>> GetByUser(string userId);

    Task<Order> Add(Order entity);
    Task Update(Order entity);
    Task Delete(int id);
    Task<IEnumerable<OrderDetail>> GetOrderDetails(int orderId);
    Task<IEnumerable<Transaction>> GetTransactions(int orderId);
}
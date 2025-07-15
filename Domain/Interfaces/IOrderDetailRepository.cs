using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderDetailRepository
{
    Task<OrderDetail> GetById(int id);
    Task<IEnumerable<OrderDetail>> GetAll();
    Task<IEnumerable<OrderDetail>> GetByOrderId(int orderId);
    Task<OrderDetail> Add(OrderDetail entity);
    Task Update(OrderDetail entity);
    Task Delete(int id);
}
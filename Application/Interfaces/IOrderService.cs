using Application.DTOs.OrderDTOs;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersForCurrentUserAsync();
    }
}

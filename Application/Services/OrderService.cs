using Application.DTOs.OrderDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public async Task<List<OrderDto>> GetOrdersForCurrentUserAsync()
        {
            var user = await _currentUserService.GetUser();
            if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado.");

            if (!int.TryParse(user.Id, out int userId))
            {
                throw new InvalidOperationException("Formato de ID de usuario inválido.");
            }

            var orders = await _orderRepository.GetByUser(user.Id);
            return _mapper.Map<List<OrderDto>>(orders);
        }

    }
}

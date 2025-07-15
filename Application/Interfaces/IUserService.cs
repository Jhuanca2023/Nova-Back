using Application.DTOs.UsersDTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetCurrentUserAsync();
    Task<UserDto> UpdateUserAsync(UserUpdateDto dto);
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);

    Task UpdateUserStatusAsync(string userId, bool isEnabled);
    Task<UserDto> UpdateUserByAdminAsync(string userId, UserUpdateDto dto);
    Task<List<Users>> ObtenerReporteAsync();
}
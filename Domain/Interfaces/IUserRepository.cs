using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Users>> GetAllUsersAsync();
    Task<Users?> GetUserByIdAsync(string userId);
    Task<Users> UpdateUserAsync(Users user);
    Task DeleteUserAsync(Users user);
    Task<bool> SaveChangesAsync();
}
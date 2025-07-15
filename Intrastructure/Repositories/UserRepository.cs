using Domain.Entities;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Users>> GetAllUsersAsync() => await _context.Users.ToListAsync();

    public async Task<Users?> GetUserByIdAsync(string userId) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<Users> UpdateUserAsync(Users user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(Users user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
}
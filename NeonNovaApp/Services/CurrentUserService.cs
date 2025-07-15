using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace NeonNovaApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly UserManager<Users> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUserService(UserManager<Users> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<Users?> GetUser()
    {
        var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            Console.WriteLine("No user ID claim found.");
            return null;
        }

        Console.WriteLine($"User ID found: {userIdClaim.Value}");
        return await _userManager.FindByIdAsync(userIdClaim.Value);
    }



}
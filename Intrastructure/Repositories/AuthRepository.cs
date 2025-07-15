using Domain.Interfaces;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Intrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signInManager;

    public AuthRepository(UserManager<Users> userManager, SignInManager<Users> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Users?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateUserAsync(Users user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(Users user, string password)
    {
        return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
    }

    public async Task<IList<Claim>> GetUserClaimsAsync(Users user)
    {
        return await _userManager.GetClaimsAsync(user);
    }

    public async Task AddClaimAsync(Users user, Claim claim)
    {
        await _userManager.AddClaimAsync(user, claim);
    }

    public async Task RemoveClaimAsync(Users user, Claim claim)
    {
        await _userManager.RemoveClaimAsync(user, claim);
    }

    public async Task AddRoleAsync(Users users, string role)
    {
        await _userManager.AddToRoleAsync(users, role);
    }
    public async Task UpdatePictureClaimAsync(Users user, string pictureUrl)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var pictureClaims = claims.Where(c => c.Type == "picture").ToList();
        foreach (var claim in pictureClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        await _userManager.AddClaimAsync(user, new Claim("picture", pictureUrl));
    }

    public async Task<bool> HasPictureClaimAsync(Users user, string pictureUrl)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Any(c => c.Type == "picture" && c.Value == pictureUrl);
    }
}

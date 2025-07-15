using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces;

public interface IAuthRepository
{
    Task<Users?> FindUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(Users user, string password);
    Task<SignInResult> CheckPasswordSignInAsync(Users user, string password);
    Task<IList<Claim>> GetUserClaimsAsync(Users user);
    Task AddClaimAsync(Users user, Claim claim);
    Task RemoveClaimAsync(Users user, Claim claim);
    Task AddRoleAsync(Users users, string role);
    Task UpdatePictureClaimAsync(Users user, string pictureUrl);
    Task<bool> HasPictureClaimAsync(Users user, string pictureUrl);
}
using System.Runtime.InteropServices;
using Application.DTOs.UsersDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Users> _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UserService(IUserRepository userRepository, UserManager<Users> userManager, IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var userDto = _mapper.Map<UserDto>(user);
            var claims = await _userManager.GetClaimsAsync(user);

            userDto.IsAdmin = claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
            userDto.IsActive = !user.LockoutEnabled ||
                               (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow);

            // Verificar si es una cuenta de Google
            userDto.IsGoogleAccount = claims.Any(c => c.Type == "isGoogleAccount" && c.Value == "true");

            var pictureClaim = claims.FirstOrDefault(c => c.Type == "picture");
            if (!string.IsNullOrEmpty(pictureClaim?.Value))
            {
                // Para imágenes de Google, usar la URL directa sin proxy
                if (pictureClaim.Value.Contains("googleusercontent.com") || pictureClaim.Value.Contains("ggpht.com"))
                {
                    userDto.AvatarUrl = pictureClaim.Value;
                }
                else
                {
                    userDto.AvatarUrl = $"/api/proxy/image?url={Uri.EscapeDataString(pictureClaim.Value)}";
                }
            }

            userDto.InitialAvatar =
                $"{user.FirstName?.Substring(0, 1 > user.FirstName?.Length ? user.FirstName?.Length ?? 0 : 1)}{user.LastName?.Substring(0, 1 > user.LastName?.Length ? user.LastName?.Length ?? 0 : 1)}"
                    .ToUpper();

            userDto.CreatedAt = user.CreatedAt;
            userDto.LastLogin = user.LastLogin;

            userDtos.Add(userDto);
        }

        return userDtos;
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) return null;

        var userDto = _mapper.Map<UserDto>(user);
        var claims = await _userManager.GetClaimsAsync(user);

        // Establecer propiedades adicionales
        userDto.IsAdmin = claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
        userDto.IsActive = !user.LockoutEnabled || (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow);
        userDto.IsGoogleAccount = claims.Any(c => c.Type == "isGoogleAccount" && c.Value == "true");

        // Configurar avatar
        var pictureClaim = claims.FirstOrDefault(c => c.Type == "picture");
        if (!string.IsNullOrEmpty(pictureClaim?.Value))
        {
            // Para imágenes de Google, usar la URL directa sin proxy
            if (pictureClaim.Value.Contains("googleusercontent.com") || pictureClaim.Value.Contains("ggpht.com"))
            {
                userDto.AvatarUrl = pictureClaim.Value;
            }
            else
            {
                userDto.AvatarUrl = $"/api/proxy/image?url={Uri.EscapeDataString(pictureClaim.Value)}";
            }
        }

        // Configurar iniciales para avatar
        userDto.InitialAvatar =
            $"{user.FirstName?.Substring(0, 1 > user.FirstName?.Length ? user.FirstName?.Length ?? 0 : 1)}{user.LastName?.Substring(0, 1 > user.LastName?.Length ? user.LastName?.Length ?? 0 : 1)}"
                .ToUpper();

        userDto.CreatedAt = user.CreatedAt;
        userDto.LastLogin = user.LastLogin;
        userDto.Phone = user.PhoneNumber!;
        return userDto;
    }

    public async Task<UserDto> UpdateUserAsync(UserUpdateDto dto)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        if (!string.IsNullOrEmpty(dto.FirstName))
            user.FirstName = dto.FirstName;

        if (!string.IsNullOrEmpty(dto.LastName))
            user.LastName = dto.LastName;

        if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != user.PhoneNumber)
        {
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, dto.Phone);
            if (!phoneResult.Succeeded)
            {
                throw new InvalidOperationException("No se pudo actualizar el número de teléfono: " +
                                                string.Join(", ", phoneResult.Errors.Select(e => e.Description)));
            }
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            bool isGoogleAccount = claims.Any(c => c.Type == "isGoogleAccount" && c.Value == "true");

            if (isGoogleAccount)
            {
                throw new InvalidOperationException(
                    "No se puede cambiar el correo electrónico de una cuenta de Google.");
            }

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new InvalidOperationException("El correo electrónico ya está en uso por otro usuario.");
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
            var result = await _userManager.ChangeEmailAsync(user, dto.Email, token);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("No se pudo actualizar el correo electrónico: " +
                                                string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.SetUserNameAsync(user, dto.Email);
        }

        await _userManager.UpdateAsync(user);
        var updatedUser = await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<UserDto> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Phone = user.PhoneNumber!;
        var claims = await _userManager.GetClaimsAsync(user);
        userDto.IsAdmin = claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
        userDto.IsActive = !user.LockoutEnabled || (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.UtcNow);
        userDto.IsGoogleAccount = claims.Any(c => c.Type == "isGoogleAccount" && c.Value == "true");
        var pictureClaim = claims.FirstOrDefault(c => c.Type == "picture");
        if (!string.IsNullOrEmpty(pictureClaim?.Value))
        {
            userDto.AvatarUrl = $"/api/proxy/image?url={Uri.EscapeDataString(pictureClaim.Value)}";
        }

        userDto.InitialAvatar =
            $"{user.FirstName?.Substring(0, 1 > user.FirstName?.Length ? user.FirstName?.Length ?? 0 : 1)}{user.LastName?.Substring(0, 1 > user.LastName?.Length ? user.LastName?.Length ?? 0 : 1)}"
                .ToUpper();

        userDto.CreatedAt = user.CreatedAt;
        userDto.LastLogin = user.LastLogin;

        return userDto;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        await _userRepository.DeleteUserAsync(user);
        return true;
    }

    public async Task UpdateUserStatusAsync(string userId, bool isEnabled)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        if (isEnabled)
        {
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
        }
        else
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        }

        await _userManager.UpdateAsync(user);
    }

    public async Task<UserDto> UpdateUserByAdminAsync(string userId, UserUpdateDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;

        if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != user.PhoneNumber)
        {
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, dto.Phone);
            if (!phoneResult.Succeeded)
            {
                throw new InvalidOperationException("No se pudo actualizar el número de teléfono: " +
                                                  string.Join(", ", phoneResult.Errors.Select(e => e.Description)));
            }
        }

        // Actualizar correo electrónico
        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            bool isGoogleAccount = claims.Any(c => c.Type == "isGoogleAccount" && c.Value == "true");
            if (isGoogleAccount)
            {
                throw new InvalidOperationException(
                    "No se puede cambiar el correo electrónico de una cuenta de Google.");
            }

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new InvalidOperationException("El correo electronico ya esta en uso por otro usuario");
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
            var result = await _userManager.ChangeEmailAsync(user, dto.Email, token);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("No se pudo actualizar el correo electrónico: " +
                                                  string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.SetUserNameAsync(user, dto.Email);
        }

        await _userManager.UpdateAsync(user);
        var updateUser = await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserDto>(updateUser);
    }

    public async Task<List<Users>> ObtenerReporteAsync()
    {
        return (await _userRepository.GetAllUsersAsync()).ToList();
    }
}
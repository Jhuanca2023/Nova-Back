using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<Users> _signInManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;


        public AuthService(IAuthRepository authRepository,
            SignInManager<Users> signInManager,
            ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _currentUserService = currentUserService;
            _userRepository = userRepository;

        }

        public async Task<AuthenticationResponseDto> Register(CredentialsUserDto dto)
        {
            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.Phone,
                EmailConfirmed = true,
                LockoutEnabled = false,
                LockoutEnd = null
            };

            var result = await _authRepository.CreateUserAsync(user, dto.Password!);
            await _authRepository.AddClaimAsync(user, new Claim("isUser", "true"));
            // await _authRepository.AddRoleAsync(user, "USER");
            if (result.Succeeded)
            {
                var loginDto = new LoginRequestDto
                {
                    Email = dto.Email,
                    Password = dto.Password
                };
                return await CreateToken(loginDto);
            }
            else
            {
                throw new Exception("Error al registrar el usuario");
            }
        }

        public async Task<AuthenticationResponseDto> Login(LoginRequestDto dto)
        {
            var user = await _authRepository.FindUserByEmailAsync(dto.Email);
            if (user is null) throw new Exception("Login Incorrecto");

            if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("Esta cuenta ha sido desactivada por un administrador.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, false);
            if (result.Succeeded)
            {
                return await CreateToken(dto);
            }
            else
            {
                throw new Exception("Login Incorrecto");
            }
        }

        public async Task<AuthenticationResponseDto> RenovateToken()
        {
            var user = await _currentUserService.GetUser();
            if (user is null) throw new Exception("Usuario no encontrado");
            if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("Esta cuenta ha sido desactivada por un administrador.");
            }
            var loginDto = new LoginRequestDto
            {
                Email = user.Email!,
                Password = string.Empty
            };

            return await CreateToken(loginDto);
        }


        public async Task<AuthenticationResponseDto> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal is null)
            {
                throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null");
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (email is null)
            {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }

            var user = await _authRepository.FindUserByEmailAsync(email);
            var pictureUrl = claimsPrincipal.FindFirstValue("picture");

            if (user != null && !string.IsNullOrEmpty(pictureUrl))
            {
                // Usar el nuevo método que elimina todos los claims duplicados y añade uno nuevo
                var hasSamePicture = await _authRepository.HasPictureClaimAsync(user, pictureUrl);
                if (!hasSamePicture)
                {
                    await _authRepository.UpdatePictureClaimAsync(user, pictureUrl);
                }
            }

            if (user is null)
            {
                var firstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? "Usuario";
                var lastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? "Google";

                user = new Users
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    LockoutEnabled = false,  
                    LockoutEnd = null,       
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _authRepository.CreateUserAsync(user, Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
                if (!result.Succeeded)
                {
                    throw new ExternalLoginProviderException("Google", "No se pudo crear el usuario");
                }

                await _authRepository.AddClaimAsync(user, new Claim("isUser", "true"));
                await _authRepository.AddClaimAsync(user, new Claim("isGoogleAccount", "true"));
                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    await _authRepository.AddClaimAsync(user, new Claim("picture", pictureUrl));
                }
            }

            if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("Esta cuenta ha sido desactivada por un administrador.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);
            var loginDto = new LoginRequestDto
            {
                Email = email,
                Password = string.Empty
            };

            return await CreateToken(loginDto);
        }

        public async Task UpdateAdminStatusAsync(string userId, bool isAdmin)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user is null) throw new KeyNotFoundException("Usuario no encontrado");
            if (isAdmin)
            {
                var adminClaim = new Claim("isAdmin", "true");
                await _authRepository.AddClaimAsync(user, adminClaim);
            }
            else
            {
                var adminClamin = new Claim("isAdmin", "true");
                await _authRepository.RemoveClaimAsync(user, adminClamin);
                var userClaims = await _authRepository.GetUserClaimsAsync(user);
                if (!userClaims.Any(c => c.Type == "isUser" && c.Value == "true"))
                {
                    await _authRepository.AddClaimAsync(user, new Claim("isUser", "true"));
                }
            }
        }

        private async Task<AuthenticationResponseDto> CreateToken(LoginRequestDto dto)
        {
            var claims = new List<Claim>
            {
                new Claim("email", dto.Email)
            };




            var user = await _authRepository.FindUserByEmailAsync(dto.Email);


            // 🟢 Agregá este claim con el ID del usuario
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user!.Id));


            var claimsDB = await _authRepository.GetUserClaimsAsync(user!);
            claims.AddRange(claimsDB);
            var keyJwt = Environment.GetEnvironmentVariable("key_jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJwt!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(3);
            var tokenSecurity = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);
            return new AuthenticationResponseDto
            {
                Token = token,
                Expired = expiration
            };
        }
    }
}
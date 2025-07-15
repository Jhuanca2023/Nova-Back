using Application.DTOs.UsersDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("current")]
    // [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetCurrentUser()
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetUserById(string userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener usuario: {ex.Message}");
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser(UserUpdateDto dto)
    {
        try
        {
            var userUpdate = await _userService.UpdateUserAsync(dto);
            return Ok(userUpdate);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar usuario: {ex.Message}");
        }
    }

    [HttpPut("admin/{userId}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> UpdateUserByAdmin(string userId, UserUpdateDto dto)
    {
        try
        {
            var userUpdate = await _userService.UpdateUserByAdminAsync(userId, dto);
            return Ok(userUpdate);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar usuario: {ex.Message}");
        }
    }

    [HttpDelete("{userId}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar usuario: {ex.Message}");
        }
    }


    [HttpPut("{userId}/status")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> UpdateStatus(string userId, [FromBody] UserStatusUpdateDto dto)
    {
        try
        {
            await _userService.UpdateUserStatusAsync(userId, dto.IsEnabled);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar estado del usuario: {ex.Message}");
        }
    }
}
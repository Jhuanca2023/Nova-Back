using Application.DTOs.Favorite;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/favorites")]
[ApiController]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetUserFavorites()
    {
        try
        {
            var favorites = await _favoriteService.GetCurrentUserFavoritesAsync();
            return Ok(favorites);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteDto>> AddFavorite([FromBody] AddFavoriteDto dto)
    {
        try
        {
            var favorite = await _favoriteService.AddFavoriteAsync(dto);
            return Ok(favorite);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        try
        {
            await _favoriteService.RemoveFavoriteAsync(id);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("toggle/{productId}")]
    public async Task<ActionResult<bool>> ToggleFavorite(int productId)
    {
        try
        {
            var isNowFavorite = await _favoriteService.ToggleFavoriteAsync(productId);
            return Ok(isNowFavorite);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("check/{productId}")]
    public async Task<ActionResult<bool>> CheckIsFavorite(int productId)
    {
        try
        {
            var isFavorite = await _favoriteService.IsProductFavoriteAsync(productId);
            return Ok(isFavorite);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
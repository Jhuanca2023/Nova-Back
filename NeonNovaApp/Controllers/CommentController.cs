using Application.DTOs.ComentDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    public class ProductCommentsController : ControllerBase
    {
        private readonly IProductCommentService _commentService;

        public ProductCommentsController(IProductCommentService commentService)
        {
            _commentService = commentService;
        }

       
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsByProductId(int productId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByProductIdAsync(productId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }
        
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddComment(int productId, [FromBody] CreateCommentDto dto)
        {
            try
            {
                var result = await _commentService.AddCommentAsync(productId, dto);
                return CreatedAtAction(nameof(AddComment), new { productId = productId, id = result.Id }, result); // 201 Created
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message }); // 401 Unauthorized
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 Bad Request
            }
        }
       
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
        {
            try
            {
                var result = await _commentService.UpdateCommentAsync(commentId, dto);
                return Ok(result); // 200 OK
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
        }

       
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(commentId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("has-commented")]
        public async Task<IActionResult> HasUserCommented([FromQuery] int productId, [FromQuery] string userId)  
        {
            try
            {
                var exists = await _commentService.HasUserCommentedAsync(productId, userId);
                return Ok(new { exists });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { message = knfEx.Message });
            }
            catch (InvalidOperationException invOpEx)
            {
                return BadRequest(new { message = invOpEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }
    }
}

using Application.DTOs.CategoryDTOs;
using Application.DTOs.Common;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<CategoryWithProductCountDto>>> GetAllCategories(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10
        )
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            var categories = await _categoryService.GetAllPaginatedAsync(pageNumber, pageSize);
            return Ok(categories);
        }

        [HttpGet("landing")]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequestDto dto)
        {
            var category = await _categoryService.AddCategoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryRequestDto dto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductResponseDTO>>> GetProductsByCategory(int id)
        {
            try
            {
                var products = await _categoryService.GetProductsByCategoryAsync(id);
                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos por categoría: {ex.Message}");
            }
        }

        [HttpGet("{id}/products-with-first-image")]
        public async Task<ActionResult<PaginatedResponseDto<ProductWithFirstImageDTO>>> GetProductsWithFirstImage(
            int id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                var products =
                    await _categoryService.GetProductsByCategoryWithFirstImagePaginatedAsync(id, pageNumber, pageSize);
                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
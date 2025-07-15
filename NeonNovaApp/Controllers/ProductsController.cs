using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("/api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;

    public ProductController(IProductService productService, IProductImageService productImageService)
    {
        _productService = productService;
        _productImageService = productImageService;
    }

    [HttpGet("simplified-admin")]
    public async Task<IActionResult> GetProductsForAdmin(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] ProductStatus? status = null,
        [FromQuery] string searchTerm = null
    )
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var products = await _productService.GetProductsFormAdmin(
            pageNumber, pageSize, categoryId, status, searchTerm);

        return Ok(products);
    }

    [HttpPost]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> Create([FromForm] CreateProductRequestDTO dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpGet("with-comments")]
    public async Task<ActionResult<ProductWithCommentsPaginatedResponseDto>> GetAllWithCommentsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ProductStatus? status = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var products = await _productService.GetAllPaginatedWithCommentsAsync(pageNumber, pageSize, status);
        return Ok(products);
    }
    [HttpPost("update-out-of-stock")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> UpdateOutOfStockProducts()
    {
        try
        {
            int updatedCount = await _productService.UpdateOutOfStockProductsAsync();
            return Ok(new { message = $"Se actualizaron {updatedCount} productos a estado 'Agotado'" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar productos", detail = ex.Message });
        }
    }
    [HttpGet]
    public async Task<ActionResult<ProductPaginatedResponseDto>> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ProductStatus? status = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var products = await _productService.GetAllPaginatedAsync(pageNumber, pageSize, status);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdWithImagesAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("simplified")]
    public async Task<IActionResult> GetSimplifiedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string searchTerm = null
    )
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var products = await _productService.GetProductSimplified(pageNumber, pageSize, categoryId, searchTerm);
        return Ok(products);
    }

    [HttpPut("{productId}/images/{imageId}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> UpdateImage(int productId, int imageId, [FromForm] UpdateProductImageDTO dto)
    {
        var updatedImage = await _productService.UpdateImageAsync(productId, imageId, dto.Image);
        return Ok(updatedImage);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        await _productImageService.AddImageAsync(id, image);
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequestDTO dto)
    {
        try
        {
            var updateProduct = await _productService.UpdateAsync(id, dto);
            return Ok(updateProduct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete("{productId}/images/{imageId}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        try
        {
            var images = await _productImageService.GetImagesProductIdAsync(productId);
            if (!images.Any(img => img.Id == imageId))
            {
                return NotFound("La imagen no pertenece al producto especificado");
            }

            await _productImageService.DeleteImageAsync(imageId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Imagen no encontrada");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar la imagen: {ex.Message}");
        }
    }

    [HttpGet("{id}/with-comments")]
    public async Task<ActionResult<ProductWithPaginatedCommentsDto>> GetProductWithComments(
        int id,
        [FromQuery] int commentsPage = 1,
        [FromQuery] int commentsPageSize = 5)
    {
        try
        {
            if (commentsPage < 1) commentsPage = 1;
            if (commentsPageSize < 1) commentsPageSize = 5;

            var product = await _productService.GetProductWithPaginatedCommentsAsync(
                id, commentsPage, commentsPageSize);

            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Error al obtener el producto con comentarios", detail = ex.Message });
        }
    }
}
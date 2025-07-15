using Application.DTOs.Favorite;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FavoriteDto>> GetCurrentUserFavoritesAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(user.Id);
    
        var favoriteDtos = new List<FavoriteDto>();
        foreach (var favorite in favorites)
        {
            // Obtener la puntuación directamente del producto o calcularla si es necesario
            int? punctuation = favorite.Product.Punctuation;
            if (punctuation == null && favorite.Product.Comments.Any())
            {
                // Calcular la puntuación si no está establecida pero hay comentarios
                punctuation = (int)Math.Round(favorite.Product.Comments.Average(c => c.Rating));
            }
        
            var dto = new FavoriteDto
            {
                Id = favorite.Product.Id,
                Name = favorite.Product.Name,
                Price = favorite.Product.Price,
                CategoryId = favorite.Product.CategoryId,
                CategoryName = favorite.Product.Category.Name,
                Status = favorite.Product.Status,
                Punctuation = punctuation, // Usar la puntuación calculada
                ImageUrl = favorite.Product.Images.FirstOrDefault()?.ImageUrl ?? ""
            };
            favoriteDtos.Add(dto);
        }

        return favoriteDtos;
    }

    public async Task<FavoriteDto> AddFavoriteAsync(AddFavoriteDto dto)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        // Verificar si el producto existe
        var product = await _productRepository.GetByIdWithCategoryAsync(dto.ProductId); // Usar GetByIdWithCategoryAsync
        if (product is null) throw new KeyNotFoundException($"Producto con ID {dto.ProductId} no encontrado");

        // Verificar si ya está en favoritos
        var existingFavorite = await _favoriteRepository.FavoriteExistsAsync(user.Id, dto.ProductId);
        if (existingFavorite) throw new InvalidOperationException("Este producto ya está en tus favoritos");

        // Crear nuevo favorito
        var favorite = new Favorite
        {
            UserId = user.Id,
            ProductId = dto.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        var addedFavorite = await _favoriteRepository.AddFavoriteAsync(favorite);
        
        // Obtener el producto completo
        var images = await _productRepository.GetProductImagesAsync(product.Id);

        // Mapear a DTO con el formato deseado
        var favoriteDto = new FavoriteDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Status = product.Status,
            Punctuation = product.Punctuation ?? 0,  
            ImageUrl = images.FirstOrDefault()?.ImageUrl ?? ""
        };

        return favoriteDto;
    }

    public async Task RemoveFavoriteAsync(int favoriteId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        var favorite = await _favoriteRepository.GetFavoriteByIdAsync(favoriteId);
        if (favorite == null) throw new KeyNotFoundException("Favorito no encontrado");
        
        // Verificar que el favorito pertenece al usuario actual
        if (favorite.UserId != user.Id) 
            throw new UnauthorizedAccessException("No tienes permiso para eliminar este favorito");

        await _favoriteRepository.RemoveFavoriteAsync(favorite);
    }
    
    public async Task<bool> ToggleFavoriteAsync(int productId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        
        // Verificar si el producto existe
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null) throw new KeyNotFoundException($"Producto con ID {productId} no encontrado");
        
        // Verificar si ya está en favoritos
        var favorite = await _favoriteRepository.GetFavoriteByUserAndProductAsync(user.Id, productId);
        
        if (favorite != null)
        {
            // Si ya existe, lo eliminamos
            await _favoriteRepository.RemoveFavoriteAsync(favorite);
            return false; // Indicamos que ya no está en favoritos
        }
        else
        {
            // Si no existe, lo creamos
            var newFavorite = new Favorite
            {
                UserId = user.Id,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _favoriteRepository.AddFavoriteAsync(newFavorite);
            return true; // Indicamos que ahora está en favoritos
        }
    }
    
    public async Task<bool> IsProductFavoriteAsync(int productId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        
        return await _favoriteRepository.FavoriteExistsAsync(user.Id, productId);
    }
}
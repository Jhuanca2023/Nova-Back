using Application.DTOs.CartShopDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class CartShopService : ICartShopService
{
    private readonly ICartShopRepository _cartShopRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private const string CartCacheKey = "UserCart_{0}";

    public CartShopService(ICartShopRepository cartShopRepository, IProductRepository productRepository,
        ICurrentUserService currentUserService, IMemoryCache memoryCache, IMapper mapper)
    {
        _cartShopRepository = cartShopRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<CartShopDto> GetCurrentCartAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        //Obtener desde cache
        var cacheKey = string.Format(CartCacheKey, user.Id);
        if (_memoryCache.TryGetValue(cacheKey, out CartShopDto cachedCart))
        {
            return cachedCart;
        }

        //Si no esta en cache obtener de la DB y guardar en la cache
        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null)
        {
            cart = new CartShop
            {
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                Status = CartShopStatus.Active
            };
            cart = await _cartShopRepository.CreateCartAsync(cart);
        }

        var cartDto = await MapCartToDto(cart);
        foreach (var detail in cartDto.Details)
        {
            var product = await _productRepository.GetByIdAsync(detail.ProductId);
            if (product != null)
            {
                detail.Stock = product.Stock;
            }
        }

        //Actualizar cache
        _memoryCache.Set(cacheKey, cartDto, TimeSpan.FromMinutes(20));
        return cartDto;
    }


    public async Task<CartShopDto> AddToCartAsync(AddToCartShopDto dto)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null)
        {
            cart = new CartShop
            {
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                Status = CartShopStatus.Active
            };
            cart = await _cartShopRepository.CreateCartAsync(cart);
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
            throw new KeyNotFoundException($"Producto {dto.ProductId} no encontrado");

        // Validar el stock
        if (product.Stock < dto.Quantity)
            throw new ApplicationException($"No hay suficiente stock para el producto '{product.Name}'");

        var existingItem = cart.CartShopDetails.FirstOrDefault(d => d.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            // Validar el stock nuevamente si el producto ya está en el carrito
            if (product.Stock < existingItem.Quantity + dto.Quantity)
                throw new ApplicationException($"No hay suficiente stock para el producto '{product.Name}'");

            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            cart.CartShopDetails.Add(new CartShopDetail
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = product.Price
            });
        }

        await _cartShopRepository.UpdateCartAsync(cart);

        // Recargar el carrito para obtener los datos completos
        cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);

        // Actualizar cache usando directamente AutoMapper
        var cartDto = _mapper.Map<CartShopDto>(cart);
        _memoryCache.Set(string.Format(CartCacheKey, user.Id), cartDto, TimeSpan.FromMinutes(10));

        return cartDto;
    }

    // public async Task<CartShopDto> UpdateCartItemAsync(UpdateCartShopItemDto dto)
    // {
    //     var user = await _currentUserService.GetUser();
    //     if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

    //     var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
    //     if (cart is null) throw new KeyNotFoundException("Carrito no encontrado");

    //     var item = cart.CartShopDetails.FirstOrDefault(d => d.Id == dto.CartDetailId);
    //     if (item is null) throw new KeyNotFoundException("Item del carrito no encontrado");

    //     // Validar que la cantidad sea positiva
    //     if (dto.Quantity <= 0)
    //         throw new ArgumentException("La cantidad debe ser mayor que cero");

    //     var product = await _productRepository.GetByIdAsync(item.ProductId);
    //     if (product == null)
    //         throw new KeyNotFoundException($"Producto {item.ProductId} no encontrado");

    //     if (product.Stock < dto.Quantity)
    //         throw new ApplicationException($"No hay suficiente stock para el producto '{product.Name}'. Stock disponible: {product.Stock}");

    //     item.Quantity = dto.Quantity;
    //     await _cartShopRepository.UpdateCartAsync(cart);

    //     // Actualizar cache
    //     var cartDto = await MapCartToDto(cart);
    //     _memoryCache.Set(string.Format(CartCacheKey, user.Id), cartDto, TimeSpan.FromMinutes(10));

    //     return cartDto;
    // }

    public async Task<CartShopDto> RemoveCartItemAsync(int cartDetailId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado");
        var item = cart.CartShopDetails.FirstOrDefault(d => d.Id == cartDetailId);
        if (item == null)
            throw new KeyNotFoundException("Item del carrito no encontrado");

        // Aquí falta llamar al método para eliminar el detalle
        await _cartShopRepository.RemoveCartDetailAsync(item);

        //Actualizar cache
        var cartDto = await MapCartToDto(cart);
        _memoryCache.Set(string.Format(CartCacheKey, user.Id), cartDto, TimeSpan.FromMinutes(20));
        return cartDto;
    }
    // Implementación del ClearCartAsync() sin parámetros
    public async Task<bool> ClearCartAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user == null) throw new UnauthorizedAccessException();
        await ClearCartAsync(user.Id);
        return true;
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(userId);
        if (cart != null)
        {
            // Eliminar todos los detalles del carrito
            await _cartShopRepository.ClearCartDetailsAsync(cart.Id);

            // Marcar el carrito como inactivo o completado
            cart.Status = CartShopStatus.Inactive;
            await _cartShopRepository.UpdateCartAsync(cart);

            // Limpiar la caché del carrito
            _memoryCache.Remove($"UserCart_{userId}");
        }
    }

    public async Task<bool> CompleteCartAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado");
        cart.Status = CartShopStatus.Completed;
        await _cartShopRepository.UpdateCartAsync(cart);
        //Eliminar cache
        _memoryCache.Remove(string.Format(CartCacheKey, user.Id));
        return true;
    }

    public async Task<CartShopDto> RefreshCartAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");
        _memoryCache.Remove(string.Format(CartCacheKey, user.Id));
        return await GetCurrentCartAsync();
    }

    private async Task<CartShopDto> MapCartToDto(CartShop cart)
    {
        return _mapper.Map<CartShopDto>(cart);
    }

    public async Task<CartShopDto> IncrementCartItemAsync(int cartDetailId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado");

        var item = cart.CartShopDetails.FirstOrDefault(d => d.Id == cartDetailId);
        if (item is null) throw new KeyNotFoundException("Item del carrito no encontrado");

        var product = await _productRepository.GetByIdAsync(item.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Producto {item.ProductId} no encontrado");

        // Verificar stock disponible
        if (product.Stock <= item.Quantity)
            throw new ApplicationException($"No hay suficiente stock para el producto '{product.Name}'. Stock disponible: {product.Stock}");

        // Incrementar en 1
        item.Quantity += 1;
        await _cartShopRepository.UpdateCartAsync(cart);

        // Actualizar cache
        var cartDto = await MapCartToDto(cart);
        _memoryCache.Set(string.Format(CartCacheKey, user.Id), cartDto, TimeSpan.FromMinutes(10));

        return cartDto;
    }

    public async Task<CartShopDto> DecrementCartItemAsync(int cartDetailId)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new UnauthorizedAccessException("Usuario no autenticado");

        var cart = await _cartShopRepository.GetActiveCartByUserIdAsync(user.Id);
        if (cart is null) throw new KeyNotFoundException("Carrito no encontrado");

        var item = cart.CartShopDetails.FirstOrDefault(d => d.Id == cartDetailId);
        if (item is null) throw new KeyNotFoundException("Item del carrito no encontrado");

        // Si la cantidad es 1, eliminar el item
        if (item.Quantity == 1)
        {
            return await RemoveCartItemAsync(cartDetailId);
        }

        // Decrementar en 1
        item.Quantity -= 1;
        await _cartShopRepository.UpdateCartAsync(cart);

        // Actualizar cache
        var cartDto = await MapCartToDto(cart);
        _memoryCache.Set(string.Format(CartCacheKey, user.Id), cartDto, TimeSpan.FromMinutes(10));

        return cartDto;
    }
}
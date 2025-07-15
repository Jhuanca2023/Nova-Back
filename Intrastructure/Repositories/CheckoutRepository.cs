using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Repositories;

public class CheckoutRepository:ICheckoutRepository
{
    private readonly ApplicationDbContext _db;

    public CheckoutRepository(ApplicationDbContext db )
    {
        _db = db;
    }
   
    public async Task<Address> SaveShippingAddressAsync(Address address)
    {
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();
        return address;
    }

    public async Task<CartShop> GetActiveCartByUserIdAsync(string userId)
    {
        return await _db.CartShops
            .Include(c => c.CartShopDetails)
            .Where(c => c.UserId == userId && c.Status == CartShopStatus.Active)
            .OrderByDescending(c => c.CreationDate)
            .FirstOrDefaultAsync();
    }



    public async Task<Address> GetLatestShippingAddressByUserIdAsync(string userId)
    {
        return await _db.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ProductExistsAsync(int productId)
    {
        return await _db.Products.AnyAsync(p => p.Id == productId);
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        return await _db.Products.FindAsync(productId);
    }

    

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }



    public async Task SaveCartAsync(CartShop cart)
    {
        _db.CartShops.Add(cart);
        await _db.SaveChangesAsync();
    }

    public async Task SaveOrderAsync(Order order)
    {
        try
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            Console.WriteLine("✅ Orden guardada correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al guardar orden: {ex.Message}");
            throw;
        }
    }


    public async Task UpdateCartStatusAsync(CartShop cart)
    {
        _db.CartShops.Update(cart);
        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
    public async Task ClearCartAsync(string userId)
    {
        // Buscar el carrito activo del usuario
        var cart = await _db.CartShops
            .Include(c => c.CartShopDetails) // Incluir los detalles del carrito
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartShopStatus.Active);

        if (cart != null)
        {
            // Opción 1: Eliminar todos los detalles del carrito
            _db.CartShopDetails.RemoveRange(cart.CartShopDetails);
            cart.Status = CartShopStatus.Inactive;
            await _db.SaveChangesAsync();
        }
    }


    // Método para actualizar el stock del producto
    public async Task UpdateProductStock(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }
}
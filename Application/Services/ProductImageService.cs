using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinary;

    public ProductImageService(IProductImageRepository repository, IMapper mapper, ICloudinaryService cloudinary)
    {
        _repository = repository;
        _mapper = mapper;
        _cloudinary = cloudinary;
    }

    public async Task<ProductImageDTO> AddImageAsync(int productId, IFormFile image)
    {
        //Subir a cloudinary
        var imageUrl = await _cloudinary.UploadImage(image);
        var publicId = _cloudinary.GetPublicIdFromUrl(imageUrl);
        //Crear la entidad 
        var newImage = new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            PublicId = publicId,
            CreateAt = DateTime.UtcNow
        };
        await _repository.AddAsync(newImage);
        return _mapper.Map<ProductImageDTO>(newImage);
    }

    public async Task DeleteImageAsync(int imageId)
    {
        var image = await _repository.GetByIdAsync(imageId);
        if (image is null) throw new KeyNotFoundException();
        //Eliminar de cloudinary
        await _cloudinary.DeleteImage(image.PublicId);
        //Eliminar de la DB
        await _repository.DeleteAsync(image);
    }

    public async Task<ProductImage> GetPrimaryImageAsync(int productId)
    {
        return await _repository.GetAllByProductId(productId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProductImageDTO>> GetImagesProductIdAsync(int productId)
    {
        var images = await _repository.GetAllByProductIdAsync(productId);
        return _mapper.Map<List<ProductImageDTO>>(images);
    }
}
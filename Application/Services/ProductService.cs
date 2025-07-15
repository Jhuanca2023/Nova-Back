using Application.DTOs.CategoryDTOs;
using Application.DTOs.Common;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IProductImageRepository _imageRepository;
    private readonly ICloudinaryService _cloudinary;
    private readonly IMapper _mapper;
    private readonly IProductImageService _productImageService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductCommentService _productCommentService;

    public ProductService(IProductRepository repository, IProductImageRepository imageRepository,
        ICloudinaryService cloudinary, IMapper mapper,
        IProductImageService productImageService,
        ICategoryRepository categoryRepository,
        IProductCommentService productCommentService
    )
    {
        _repository = repository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _productImageService = productImageService;
        _categoryRepository = categoryRepository;
        _productCommentService = productCommentService;
    }


    public async Task<List<ProductResponseDTO>> GetAllAsync()
    {
        var products = await _repository.GetAllWithCategoriesAsync();
        var productDTOs = new List<ProductResponseDTO>();

        foreach (var product in products)
        {
            var dto = new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                Category = _mapper.Map<CategoryDto>(product.Category),
                Images = new List<ProductImageDTO>()
            };

            var images = await _productImageService.GetImagesProductIdAsync(product.Id);
            dto.Images = _mapper.Map<List<ProductImageDTO>>(images);
            productDTOs.Add(dto);
        }

        return productDTOs;
    }

    public async Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null) throw new KeyNotFoundException($"La categoria con ID {dto.CategoryId} no existe");
        // Crea una versión del DTO sin la propiedad Images
        var productToCreate = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Status = dto.Status,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(productToCreate);

        // Subir imágenes a Cloudinary
        if (dto.Images != null)
        {
            foreach (var image in dto.Images)
            {
                await _productImageService.AddImageAsync(productToCreate.Id, image);
            }
        }

        return await GetByIdWithImagesAsync(productToCreate.Id);
    }

    public async Task<ProductPaginatedResponseDto> GetAllPaginatedAsync(int pageNumber, int pageSize,
        ProductStatus? status = null)
    {
        var pagedResult = await _repository.GetAllPaginatedAsync(pageNumber, pageSize, status);
        var productDTOs = new List<ProductResponseDTO>();

        foreach (var product in pagedResult.Items)
        {
            var dto = new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                Punctuation = product.Punctuation,
                CreatedAt = product.CreatedAt,
                Category = _mapper.Map<CategoryDto>(product.Category),
                Images = new List<ProductImageDTO>()
            };

            var images = await _productImageService.GetImagesProductIdAsync(product.Id);
            dto.Images = _mapper.Map<List<ProductImageDTO>>(images);
            productDTOs.Add(dto);
        }

        return new ProductPaginatedResponseDto
        {
            Products = productDTOs,
            TotalItems = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };
    }

    public async Task<ProductWithCommentsPaginatedResponseDto> GetAllPaginatedWithCommentsAsync(
        int pageNumber, int pageSize, ProductStatus? status = null)
    {
        var pagedResult = await _repository.GetAllPaginatedAsync(pageNumber, pageSize, status);
        var productDTOs = new List<ProductsWithCommentsDto>();

        foreach (var product in pagedResult.Items)
        {
            var dto = new ProductsWithCommentsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                Category = _mapper.Map<CategoryDto>(product.Category),
                Images = new List<ProductImageDTO>()
            };

            var images = await _productImageService.GetImagesProductIdAsync(product.Id);
            dto.Images = _mapper.Map<List<ProductImageDTO>>(images);

            var comments = await _productCommentService.GetCommentsByProductIdAsync(product.Id);
            dto.Comments = comments;

            productDTOs.Add(dto);
        }

        return new ProductWithCommentsPaginatedResponseDto
        {
            Products = productDTOs,
            TotalItems = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };
    }

    public async Task<ProductResponseDTO> GetByIdWithImagesAsync(int id)
    {
        var product = await _repository.GetByIdWithCategoryAsync(id);
        if (product is null)
        {
            throw new KeyNotFoundException("Producto no encontrado");
        }

        var images = await _productImageService.GetImagesProductIdAsync(id);

        return new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Status = product.Status,
            Punctuation = product.Punctuation,
            Category = _mapper.Map<CategoryDto>(product.Category),
            CreatedAt = product.CreatedAt,
            Images = _mapper.Map<List<ProductImageDTO>>(images)
        };
    }

    public async Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null) throw new KeyNotFoundException();
        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Price != null) product.Price = dto.Price.Value;
        if (dto.Status != null) product.Status = dto.Status.Value;
        if (dto.Stock != null) product.Stock = dto.Stock.Value;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.CategoryId != null && dto.CategoryId != product.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (category is null) throw new KeyNotFoundException($"La categoria con ID {dto.CategoryId} no existe");
            product.CategoryId = dto.CategoryId.Value;
        }

        await _repository.UpdateAsync(product);
        // Actualizar Imagen
        if (dto.Image != null)
        {
            var oldImage = await _productImageService.GetPrimaryImageAsync(id);
            if (oldImage != null)
            {
                await _productImageService.DeleteImageAsync(oldImage.Id);
            }

            //Subir la nueva imagen
            await _productImageService.AddImageAsync(id, dto.Image);
        }

        return await GetByIdWithImagesAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null) throw new KeyNotFoundException();
        var images = await _productImageService.GetImagesProductIdAsync(id);
        foreach (var image in images)
        {
            await _productImageService.DeleteImageAsync(image.Id);
        }

        await _repository.DeletedAsync(id);
    }

    public async Task<ProductImageDTO> UpdateImageAsync(int productId, int imageId, IFormFile image)
    {
        var existingImage = await _imageRepository.GetByIdAsync(imageId);

        if (existingImage == null || existingImage.ProductId != productId)
            throw new KeyNotFoundException("Imagen no encontrada o no pertenece al producto indicado");

        await _cloudinary.DeleteImage(existingImage.PublicId);

        var imageUrl = await _cloudinary.UploadImage(image);
        var publicId = _cloudinary.GetPublicIdFromUrl(imageUrl);

        existingImage.ImageUrl = imageUrl;
        existingImage.PublicId = publicId;

        await _imageRepository.UpdateAsync(existingImage);

        return _mapper.Map<ProductImageDTO>(existingImage);
    }


    public async Task<PaginatedResponseDto<ProductoSimplificadoDto>> GetProductSimplified(
     int pageNumber,
     int pageSize,
     int? categoryId = null,
     string searchTerm = null)
    {
        var simplifiedProducts = await _repository.GetAllProductSimplifiedPaginatedAsync(pageNumber, pageSize, categoryId, searchTerm);

        var productDtos = simplifiedProducts.Items.Select(p => new ProductoSimplificadoDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.CategoryName,
            Punctuation = p.Punctuation,
            Status = p.Status,
            ImageUrl = p.ImageUrl
        }).ToList();

        return new PaginatedResponseDto<ProductoSimplificadoDto>
        {
            Items = productDtos,
            TotalItems = simplifiedProducts.TotalCount,
            PageNumber = simplifiedProducts.PageNumber,
            PageSize = simplifiedProducts.PageSize,
            TotalPages = simplifiedProducts.TotalPages
        };
    }

    public async Task<ProductWithPaginatedCommentsDto> GetProductWithPaginatedCommentsAsync(
        int productId, int commentsPage, int commentsPageSize)
    {
        var product = await _repository.GetByIdWithCategoryAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"No se encontró el producto con ID {productId}");

        var images = await _productImageService.GetImagesProductIdAsync(product.Id);
        var pagedComments = await _productCommentService.GetPaginatedCommentsByProductIdAsync(
            productId, commentsPage, commentsPageSize);


        int? punctuation = product.Punctuation;
        if (!punctuation.HasValue && pagedComments.Items.Any())
        {
            punctuation = (int)Math.Round(pagedComments.Items.Average(c => c.Rating));
        }

        return new ProductWithPaginatedCommentsDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Status = product.Status,
            Punctuation = punctuation,
            Category = _mapper.Map<CategoryDto>(product.Category),
            CreatedAt = product.CreatedAt,
            Images = _mapper.Map<List<ProductImageDTO>>(images),
            Comments = pagedComments.Items.ToList(),
            TotalCommentsCount = pagedComments.TotalCount,
            CommentsPageNumber = pagedComments.PageNumber,
            CommentsPageSize = pagedComments.PageSize,
            CommentsTotalPages = pagedComments.TotalPages
        };
    }

    public async Task<PaginatedResponseDto<ProductoSimplificadoDto>> GetProductsFormAdmin(
    int pageNumber,
    int pageSize,
    int? categoryId = null,
    ProductStatus? status = default,
    string searchTerm = null)
    {
        var simplifiedProducts = await _repository.GetProductsForAdminAsync(
            pageNumber, pageSize, categoryId, status, searchTerm);

        var productDtos = simplifiedProducts.Items.Select(p => new ProductoSimplificadoDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.CategoryName,
            Punctuation = p.Punctuation,
            Status = p.Stock <= 0 ? ProductStatus.OutOfStock : p.Status,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl
        }).ToList();

        return new PaginatedResponseDto<ProductoSimplificadoDto>
        {
            Items = productDtos,
            TotalItems = simplifiedProducts.TotalCount,
            PageNumber = simplifiedProducts.PageNumber,
            PageSize = simplifiedProducts.PageSize,
            TotalPages = simplifiedProducts.TotalPages
        };
    }
    public async Task<int> UpdateOutOfStockProductsAsync()
    {
        return await _repository.UpdateOutOfStockProductsAsync();
    }
}
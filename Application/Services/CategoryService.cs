using Application.DTOs.CategoryDTOs;
using Application.DTOs.Common;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _repository;
    private readonly IProductImageService _productImageService;
    private readonly IProductRepository _productRepository;

    public CategoryService(IMapper mapper, ICategoryRepository repository, IProductImageService productImageService,
        IProductRepository productRepository)
    {
        _mapper = mapper;
        _repository = repository;
        _productImageService = productImageService;
        _productRepository = productRepository;
    }

    public async Task<List<CategoryDto>> GetAllAsync() =>
        _mapper.Map<List<CategoryDto>>(await _repository.GetAllAsync());

    public async Task<PaginatedResponseDto<CategoryWithProductCountDto>> GetAllPaginatedAsync(int pageNumber,
        int pageSize)
    {
        var pagedResult = await _repository.GetAllPaginatedAsync(pageNumber, pageSize);

        var dtoItems = pagedResult.Items.Select(category => new CategoryWithProductCountDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            ProductCount = category.Products?.Count ?? 0
        }).ToList();

        return new PaginatedResponseDto<CategoryWithProductCountDto>
        {
            Items = dtoItems,
            TotalItems = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };
    }

    public async Task<CategoryDto> GetByIdAsync(int id) => _mapper.Map<CategoryDto>(await _repository.GetByIdAsync(id));

    public async Task<CategoryDto> AddCategoryAsync(CreateCategoryRequestDto category)
    {
        var newCategory = await _repository.AddAsync(_mapper.Map<Category>(category));
        return _mapper.Map<CategoryDto>(newCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto category)
    {
        var categoryDb = await _repository.GetByIdAsync(id);
        if (categoryDb == null)
            throw new KeyNotFoundException($"Categoria con ID {id} no encontrado");

        _mapper.Map(category, categoryDb);

        var categoryUpdated = await _repository.UpdateAsync(categoryDb);
        return _mapper.Map<CategoryDto>(categoryUpdated);
    }

    public async Task<List<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId)
    {
        var category = await _repository.GetByIdAsync(categoryId);
        if (category is null) throw new KeyNotFoundException($"La categoria con ID {categoryId} no existe");
        var products = await _productRepository.GetAllWithCategoriesAsync();
        var filteredProducts = products.Where(p => p.CategoryId == categoryId);
        var productoDTOs = new List<ProductResponseDTO>();
        foreach (var product in filteredProducts)
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
            productoDTOs.Add(dto);
        }

        return productoDTOs;
    }

    public async Task<PaginatedResponseDto<ProductWithFirstImageDTO>> GetProductsByCategoryWithFirstImagePaginatedAsync(
        int categoryId, int pageNumber, int pageSize)
    {
        var category = await _repository.GetByIdAsync(categoryId);
        if (category == null)
            throw new KeyNotFoundException($"Categoría con ID {categoryId} no encontrada");

        var pagedProducts =
            await _productRepository.GetProductsByCategoryPaginatedAsync(categoryId, pageNumber, pageSize);

        var productDTOs = new List<ProductWithFirstImageDTO>();

        foreach (var product in pagedProducts.Items)
        {
            var images = await _productImageService.GetImagesProductIdAsync(product.Id);
            var firstImage = images.FirstOrDefault();

            productDTOs.Add(new ProductWithFirstImageDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                Punctuation = product.Punctuation,
                Category = _mapper.Map<CategoryDto>(product.Category),
                FirstImage = firstImage != null ? _mapper.Map<ProductImageDTO>(firstImage) : null
            });
        }

        return new PaginatedResponseDto<ProductWithFirstImageDTO>
        {
            Items = productDTOs,
            TotalItems = pagedProducts.TotalCount,
            PageNumber = pagedProducts.PageNumber,
            PageSize = pagedProducts.PageSize,
            TotalPages = pagedProducts.TotalPages
        };
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
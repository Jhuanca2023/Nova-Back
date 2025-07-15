using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs;
using AutoMapper;
using Domain.Entities;


namespace Application.Mappings;

public class MappingProduct : Profile
{
    public MappingProduct()
    {
        // Mapear de Product hacia ProductDTO 
        CreateMap<CreateProductRequestDTO, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            ;
        CreateMap<Product, ProductResponseDTO>()
            ;

        CreateMap<ProductImage, ProductImageDTO>();
        CreateMap<Category, CategoryDto>();
    }
}
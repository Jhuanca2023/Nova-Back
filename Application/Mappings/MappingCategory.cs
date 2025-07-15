using Application.DTOs.CategoryDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingCategory : Profile
{
    public MappingCategory()
    {
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<UpdateCategoryRequestDto, Category>();
        CreateMap<CategoryDto, Category>().ReverseMap();
        CreateMap<Category, CategoryWithProductCountDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
    }
}
using Application.DTOs.DashboardDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingDashboard : Profile
{
    public MappingDashboard()
    {
        CreateMap<Product, LowStockProductDto>();
        CreateMap<Category, CategoryUsageDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
    }
}
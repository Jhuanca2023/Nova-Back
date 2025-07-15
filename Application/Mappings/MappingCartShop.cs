using Application.DTOs.CartShopDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingCartShop : Profile
{
    public MappingCartShop()
    {
        CreateMap<CartShop, CartShopDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.CartShopDetails));

        CreateMap<CartShopDetail, CartShopDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                src.Product.Images.FirstOrDefault()!.ImageUrl ?? ""));
    }
}
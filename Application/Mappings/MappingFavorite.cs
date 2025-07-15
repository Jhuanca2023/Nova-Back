using Application.DTOs.Favorite;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingFavorite : Profile
{
    public MappingFavorite()
    {
        CreateMap<Favorite, FavoriteDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Product.Status))
            .ForMember(dest => dest.Punctuation, opt => opt.MapFrom(src => src.Product.Punctuation))
            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src => src.Product.Images.FirstOrDefault() != null ?
                    src.Product.Images.FirstOrDefault().ImageUrl : ""));
    }
}
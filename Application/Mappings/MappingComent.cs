using AutoMapper;
using Application.DTOs.ComentDTOs;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingComent : Profile
    {
        public MappingComent()
        {
            CreateMap<CreateCommentDto, ProductComment>();
            CreateMap<UpdateCommentDto, ProductComment>();
            CreateMap<ProductComment, CommentDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());
        }
    }
}

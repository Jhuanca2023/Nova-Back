using Application.DTOs.UsersDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingUser : Profile
{
    public MappingUser()
    {
        CreateMap<Users, UserDto>();
    }
}
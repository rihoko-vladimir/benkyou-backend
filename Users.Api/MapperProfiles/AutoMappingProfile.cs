using AutoMapper;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.MapperProfiles;

public class AutoMappingProfile : Profile 
{
    public AutoMappingProfile()
    {
        CreateMap<UserInformation, UpdateUserInfoRequest>();
        CreateMap<UpdateUserInfoRequest, UserInformation>();
    }
}
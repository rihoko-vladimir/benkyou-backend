using AutoMapper;
using Shared.Models.Messages;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.Common.MapperProfiles;

public class AutoMappingProfile : Profile 
{
    public AutoMappingProfile()
    {
        CreateMap<UserInformation, UpdateUserInfoRequest>()
            .ReverseMap();
        CreateMap<RegisterUserMessage, UserInformation>()
            .ForMember(
                information => information.Id, 
                expression => expression.MapFrom(message => message.UserId));
    }
}
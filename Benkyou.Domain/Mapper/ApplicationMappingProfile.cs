using AutoMapper;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;

namespace Benkyou.Domain.Mapper;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<RegisterModel, User>()
            .ForMember(user => user.UserName, t => t.MapFrom(registerModel => registerModel.UserName))
            .ForMember(user => user.FirstName, t => t.MapFrom(registerModel => registerModel.FirstName))
            .ForMember(user => user.LastName, t => t.MapFrom(registerModel => registerModel.LastName))
            .ForMember(user => user.Email, t => t.MapFrom(registerModel => registerModel.Email))
            .ForMember(user => user.IsTermsAccepted, t => t.MapFrom(registerModel => registerModel.IsTermsAccepted));
        CreateMap<Kunyomi, KunyomiResponse>();
        CreateMap<Onyomi, OnyomiResponse>();
        CreateMap<Kanji, KanjiResponse>();
        CreateMap<Set, SetResponse>().ForMember(response => response.AuthorId, t => t.MapFrom(set => set.UserId));
        CreateMap<KunyomiResponse, Kunyomi>();
        CreateMap<OnyomiResponse, Onyomi>();
        CreateMap<KunyomiRequest, Kunyomi>();
        CreateMap<KanjiResponse, Kanji>();
        CreateMap<OnyomiRequest, Onyomi>();
        CreateMap<KanjiRequest, Kanji>();
        CreateMap<User, UserResponse>()
            .ForMember(response => response.Birthday,
                t => t
                    .MapFrom(user => user.Birthday != null ? user.Birthday.Value.ToString("yyyy-MM-dd") : ""))
            .ForMember(response => response.IsPublic,
                t => t.MapFrom(user => user.IsAccountPublic));
    }
}
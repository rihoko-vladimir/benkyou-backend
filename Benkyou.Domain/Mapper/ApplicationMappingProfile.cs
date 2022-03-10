using AutoMapper;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Domain.Mapper;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<RegisterModel, User>()
            .ForMember(user => user.UserName, t => t.MapFrom(registerModel => registerModel.Login))
            .ForMember(user => user.FirstName, t => t.MapFrom(registerModel => registerModel.FirstName))
            .ForMember(user => user.LastName, t => t.MapFrom(registerModel => registerModel.LastName))
            .ForMember(user => user.Email, t => t.MapFrom(registerModel => registerModel.Email))
            .ForMember(user => user.IsTermsAccepted, t => t.MapFrom(registerModel => registerModel.IsTermsAccepted));
        CreateMap<Kunyomi, KunyomiResult>();
        CreateMap<Onyomi, OnyomiResult>();
        CreateMap<Kanji, KanjiResponse>();
        CreateMap<Card, CardResponse>();
        CreateMap<KunyomiResult, Kunyomi>();
        CreateMap<OnyomiResult, Onyomi>();
        CreateMap<KunyomiRequest, Kunyomi>();
        CreateMap<KanjiResponse, Kanji>();
        CreateMap<OnyomiRequest, Onyomi>();
        CreateMap<KanjiRequest, Kanji>();
    }
}
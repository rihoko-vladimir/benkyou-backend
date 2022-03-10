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
            // .ForMember(result => result.Reading, t => t.MapFrom(kunyomi => kunyomi.Reading));
        // CreateMap<List<Kunyomi>, List<KunyomiResult>>();
        CreateMap<Onyomi, OnyomiResult>();
            // .ForMember(result => result.Reading, t => t.MapFrom(onyomi => onyomi.Reading));
        
        // CreateMap<List<Onyomi>, List<OnyomiResult>>();
        CreateMap<Kanji, KanjiResponse>();
            // .ForMember(result => result.KanjiChar, t => t.MapFrom(kanji => kanji.KanjiChar))
            // .ForMember(result => result.KunyomiReadings, t => t.MapFrom(kanji => kanji.KunyomiReadings))
            // .ForMember(result => result.OnyomiReadings, t => t.MapFrom(kanji => kanji.OnyomiReadings));
        // CreateMap<List<Kanji>, List<KanjiResponse>>();
        CreateMap<Card, CardResponse>().ForMember(response => response.Id, t => t.MapFrom(card => card.Id));
            // .ForMember(response => response.Name, t => t.MapFrom(card => card.Name))
            // .ForMember(response => response.Description, t => t.MapFrom(card => card.Description))
            // .ForMember(response => response.KanjiList, t => t.MapFrom(card => card.KanjiList));
        // CreateMap<List<Card>, List<CardResponse>>();
    }
}
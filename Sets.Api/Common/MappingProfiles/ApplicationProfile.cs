using AutoMapper;
using Sets.Api.Models.Entities;
using Sets.Api.Models.Requests;
using Sets.Api.Models.Responses;

namespace Sets.Api.Common.MappingProfiles;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<KunyomiRequest, Kunyomi>()
            .ReverseMap();
        CreateMap<OnyomiRequest, Onyomi>()
            .ReverseMap();
        CreateMap<KanjiRequest, Kanji>()
            .ReverseMap();
        CreateMap<SetRequest, Set>()
            .ReverseMap();
        CreateMap<Kunyomi, KunyomiResponse>();
        CreateMap<Onyomi, OnyomiResponse>();
        CreateMap<Kanji, KanjiResponse>();
        CreateMap<Set, SetResponse>();
    }
}
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
        CreateMap<Kunyomi, KunyomiResponse>()
            .ReverseMap();
        CreateMap<Onyomi, OnyomiResponse>()
            .ReverseMap();
        CreateMap<Kanji, KanjiResponse>()
            .ReverseMap();
        CreateMap<Set, SetResponse>()
            .ForMember(
                response =>
                    response.AuthorId,
                expression =>
                    expression.MapFrom(
                        set =>
                            set.UserId));
        CreateMap<SetResponse, Set>()
            .ForMember(
                set => set.UserId,
                expression => expression.MapFrom(
                    response => response.AuthorId));
    }
}
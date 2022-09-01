using AutoMapper;
using Shared.Models.Messages;
using Shared.Models.Models;
using Statistics.Api.Models.Entities;

namespace Statistics.Api.Common.MapperProfiles;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<KanjiResult, StudyResult>();
    }
}
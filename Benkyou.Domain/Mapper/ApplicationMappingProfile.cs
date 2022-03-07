﻿using AutoMapper;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Domain.Mapper;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<RegisterModel, User>()
            .ForMember(user => user.UserName, t => t.MapFrom(registerModel => registerModel.Login))
            .ForMember(user => user.Email, t => t.MapFrom(registerModel => registerModel.Email))
            .ForMember(user => user.IsTermsAccepted, t => t.MapFrom(registerModel => registerModel.IsTermsAccepted));
    }
}
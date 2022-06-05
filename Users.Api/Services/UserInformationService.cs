using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Shared.Models.Models;
using Users.Api.Interfaces.Repositories;
using Users.Api.Interfaces.Services;
using Users.Api.Models;
using Users.Api.Models.Requests;

namespace Users.Api.Services;

public class UserInformationService : IUserInformationService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IMapper _mapper;

    public UserInformationService(IUserInfoRepository userInfoRepository, IMapper mapper)
    {
        _userInfoRepository = userInfoRepository;
        _mapper = mapper;
    }
    

    public async Task<Result> UpdateUserInfo(JsonPatchDocument<UpdateUserInfoRequest> updateRequest, Guid userId)
    {
        var user = await _userInfoRepository.GetUserInfoAsync(userId);

        var updateDto = _mapper.Map<UpdateUserInfoRequest>(user);
        updateRequest.ApplyTo(updateDto);
        //TODO implement mapping with checks
        await _userInfoRepository.UpdateUserInfoAsync(newUser);
    }
}
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.JsonPatch;
using Serilog;
using Shared.Models.Models;
using Users.Api.Interfaces.Repositories;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Configurations;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.Services;

public class UserInformationService : IUserInformationService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IMapper _mapper;
    private readonly AzureBlobConfiguration _blobConfiguration;

    public UserInformationService(IUserInfoRepository userInfoRepository, IMapper mapper, AzureBlobConfiguration blobConfiguration)
    {
        _userInfoRepository = userInfoRepository;
        _mapper = mapper;
        _blobConfiguration = blobConfiguration;
    }
    

    public async Task<Result> UpdateUserInfoAsync(JsonPatchDocument<UpdateUserInfoRequest> updateRequest, Guid userId)
    {
        try
        {
            var user = await _userInfoRepository.GetUserInfoAsync(userId);

            var updateDto = _mapper.Map<UpdateUserInfoRequest>(user);
            updateRequest.ApplyTo(updateDto);
        
            await _userInfoRepository.UpdateUserInfoAsync(updateDto, user.Id);
            
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An error occured while trying to update user's information. Exception : {Exception}, stacktrace: {Stacktrace}", e.GetType().FullName, e.StackTrace);
            return Result.Error(e);
        }
    }

    public async Task<Result> UpdateUserAvatarAsync(IFormFile file, Guid userId)
    {
        try
        {
            var serviceClient = new BlobServiceClient(_blobConfiguration.ConnectionString);
            if (!await serviceClient.GetBlobContainerClient(_blobConfiguration.ContainerName).ExistsAsync())
            {
                await serviceClient.CreateBlobContainerAsync(_blobConfiguration.ContainerName, PublicAccessType.BlobContainer);
            }
            var fileName = $"{Guid.NewGuid()}.png";
            var blobClient = new BlobClient(_blobConfiguration.ConnectionString, _blobConfiguration.ContainerName,
                fileName);
            await using var fileStream = file.OpenReadStream();
            await blobClient.UploadAsync(fileStream);
            Log.Debug("Uploaded avatar to the server. Url is {Url}", blobClient.Uri);
            await _userInfoRepository.UpdateUserAvatarUrl(blobClient.Uri.ToString(), userId);
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An error occured while uploading avatar. Exception : {Exception}, Stacktrace: {StackTrace}", e.GetType().FullName, e.StackTrace);
            return Result.Error(e);
        }
    }

    public async Task<Result> CreateUserAsync(UserInformation userInformation)
    {
        try
        {
            await _userInfoRepository.CreateUserAsync(userInformation);
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An error occured while creating user. Exception : {Exception}, Stacktrace: {StackTrace}", e.GetType().FullName, e.StackTrace);
            return Result.Error(e);
        }
    }
}
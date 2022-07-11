using AutoMapper;
using Serilog;
using Sets.Api.Interfaces.Repositories;
using Sets.Api.Interfaces.Services;
using Sets.Api.Models.Entities;
using Sets.Api.Models.Requests;
using Sets.Api.Models.Responses;
using Shared.Models.Models;

namespace Sets.Api.Services;

public class SetsService : ISetsService
{
    private readonly ISetsRepository _setsRepository;
    private readonly IMapper _mapper;

    public SetsService(
        ISetsRepository setsRepository,
        IMapper mapper)
    {
        _setsRepository = setsRepository;
        _mapper = mapper;
    }

    public async Task<Result<SetResponse>> CreateSetAsync(Guid userId, SetRequest set)
    {
        var mappedSet = _mapper.Map<Set>(set);

        mappedSet.UserId = userId;

        var result = await _setsRepository.CreateSetAsync(mappedSet);

        if (!result.IsSuccess) return Result.Error<SetResponse>(result.Message); 
            
        Log.Information("Created new set with id: {Id} for user : {UserId}", result.Value!.Id, result.Value!.UserId);
        
        return Result.Success(_mapper.Map<SetResponse>(result.Value));
    }

    public async Task<Result<SetResponse>> PatchSetAsync(Guid userId, Guid setId, Set setValue)
    {
        var patchResult = await _setsRepository.PatchSetAsync(setValue);
        
        if (!patchResult.IsSuccess) return Result.Error<SetResponse>(patchResult.Message);
        
        var response = _mapper.Map<SetResponse>(patchResult.Value!);
        
        Log.Information("Patched set: {SetId}", setId);
        
        return Result.Success(response);
    }

    public async Task<Result> RemoveSetAsync(Guid userId, Guid setId)
    {
        var setResult = await GetSetAsync(setId);

        if (!setResult.IsSuccess)
        {
            Log.Warning("No set with id : {Id} was found", setId);
            
            return Result.Error(setResult.Message);
        }
        
        if (setResult.Value!.UserId != userId) return Result.Error("You can not remove sets of other users!");
        
        var result = await _setsRepository.RemoveSetAsync(setId);
        
        Log.Information("Removed set {SetId}", setId);
        
        return result;
    }

    public async Task<Result<PagedSetsResponse>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize)
    {
        var setsResult = await _setsRepository.GetUserSetsAsync(userId, pageNumber, pageSize);
        
        if (!setsResult.IsSuccess) return Result.Error<PagedSetsResponse>(setsResult.Message);
        
        var mappedSets = _mapper.Map<List<SetResponse>>(setsResult.Value);
        var mySetsCount = await _setsRepository.GetUserSetsPagesCountAsync(userId, pageSize);
        
        var pagedSetResponse = new PagedSetsResponse
        {
            Sets = mappedSets,
            CurrentPage = pageNumber,
            PagesCount = mySetsCount,
            SetsCount = pageSize
        };
        
        Log.Information("Finished querying user sets for user {UserId}", userId);
        
        return Result.Success(pagedSetResponse);
    }

    public async Task<Result<PagedSetsResponse>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize, string searchQuery)
    {
        var setsResult = await _setsRepository.GetAllSetsAsync(userId, pageNumber, pageSize, searchQuery);
        
        if (!setsResult.IsSuccess) return Result.Error<PagedSetsResponse>(setsResult.Message);
        
        var mappedSets = _mapper.Map<List<SetResponse>>(setsResult.Value);
        var allCount = await _setsRepository.GetAllSetsPagesCountAsync(userId, pageSize);
        
        var pagedSetResponse = new PagedSetsResponse
        {
            Sets = mappedSets,
            CurrentPage = pageNumber,
            PagesCount = allCount,
            SetsCount = pageSize
        };
        
        Log.Information("Finished querying all sets for user {UserId}", userId);
        
        return Result.Success(pagedSetResponse);
    }

    public async Task<Result<Set>> GetSetAsync(Guid setId)
    {
        var setResult = await _setsRepository.GetSetAsync(setId);
        
        if (!setResult.IsSuccess) return Result.Error<Set>(setResult.Message);

        Log.Information("Queried set with id {SetId}", setId);
        
        return Result.Success(setResult.Value!);
    }

    public async Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic)
    {
        Log.Information("Changing visibility for user {UserId}, isPublic: {IsPublic}", userId, arePublic);
        
        return await _setsRepository.ChangeSetsVisibilityAsync(userId, arePublic);
    }
}
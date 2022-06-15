using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
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
    private readonly IValidator<SetRequest> _setRequestValidator;

    public SetsService(
        ISetsRepository setsRepository,
        IMapper mapper,
        IValidator<SetRequest> setRequestValidator)
    {
        _setsRepository = setsRepository;
        _mapper = mapper;
        _setRequestValidator = setRequestValidator;
    }

    public async Task<Result<SetResponse>> CreateSetAsync(Guid userId, SetRequest set)
    {
        var validationResult = await _setRequestValidator.ValidateAsync(set);
        
        if (!validationResult.IsValid) return Result.Error<SetResponse>(validationResult.ToString("~"));

        var mappedSet = _mapper.Map<Set>(set);

        mappedSet.UserId = userId;

        var result = await _setsRepository.CreateSetAsync(mappedSet);

        return !result.IsSuccess ? Result.Error<SetResponse>(result.Message) : Result.Success(_mapper.Map<SetResponse>(result.Value));
    }

    public async Task<Result<SetResponse>> PatchSetAsync(Guid userId, Guid setId, JsonPatchDocument<SetRequest> set)
    {
        var setResult = await _setsRepository.GetSetAsync(setId);
        
        if (!setResult.IsSuccess) return Result.Error<SetResponse>(setResult.Message);
        
        var setValue = setResult.Value!;
        var setRequestDto = _mapper.Map<SetRequest>(setValue);
        set.ApplyTo(setRequestDto);

        var validationResult = await _setRequestValidator.ValidateAsync(setRequestDto);
        
        if (!validationResult.IsValid) return Result.Error<SetResponse>(validationResult.ToString("~"));
            
        setValue.Name = setRequestDto.Name;
        setValue.Description = setRequestDto.Description;
        setValue.KanjiList = _mapper.Map<List<Kanji>>(setRequestDto.KanjiList);
        
        var patchResult = await _setsRepository.PatchSetAsync(setValue);
        
        if (!patchResult.IsSuccess) return Result.Error<SetResponse>(patchResult.Message);
        
        var response = _mapper.Map<SetResponse>(patchResult.Value!);
        
        return Result.Success(response);
    }

    public async Task<Result> RemoveSetAsync(Guid userId, Guid setId)
    {
        var setResult = await GetSet(setId);
        
        if (!setResult.IsSuccess) return Result.Error(setResult.Message);
        
        if (setResult.Value!.AuthorId != userId) return Result.Error("You can not remove sets of other users!");
        
        var result = await _setsRepository.RemoveSetAsync(setId);
        
        return result;
    }

    public async Task<Result<PagedSetsResponse>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize)
    {
        var setsResult = await _setsRepository.GetUserSetsAsync(userId, pageNumber, pageSize);
        
        if (!setsResult.IsSuccess) return Result.Error<PagedSetsResponse>(setsResult.Message);
        
        var mappedSets = _mapper.Map<List<SetResponse>>(setsResult.Value);
        var mySetsCount = await _setsRepository.GetUserSetsPagesCountAsync(userId);
        
        var pagedSetResponse = new PagedSetsResponse
        {
            Sets = mappedSets,
            CurrentPage = pageNumber,
            PagesCount = mySetsCount,
            SetsCount = pageSize
        };
        
        return Result.Success(pagedSetResponse);
    }

    public async Task<Result<PagedSetsResponse>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize, string searchQuery)
    {
        var setsResult = await _setsRepository.GetAllSetsAsync(userId, pageNumber, pageSize, searchQuery);
        
        if (!setsResult.IsSuccess) return Result.Error<PagedSetsResponse>(setsResult.Message);
        
        var mappedSets = _mapper.Map<List<SetResponse>>(setsResult.Value);
        var allCount = await _setsRepository.GetAllSetsPagesCountAsync(userId);
        
        var pagedSetResponse = new PagedSetsResponse
        {
            Sets = mappedSets,
            CurrentPage = pageNumber,
            PagesCount = allCount,
            SetsCount = pageSize
        };
        
        return Result.Success(pagedSetResponse);
    }

    public async Task<Result<SetResponse>> GetSet(Guid setId)
    {
        var setResult = await _setsRepository.GetSetAsync(setId);
        
        if (!setResult.IsSuccess) return Result.Error<SetResponse>(setResult.Message);
        
        var mappedSet = _mapper.Map<SetResponse>(setResult.Value);
        
        return Result.Success(mappedSet);
    }

    public async Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic)
    {
        return await _setsRepository.ChangeSetsVisibilityAsync(userId, arePublic);
    }
}
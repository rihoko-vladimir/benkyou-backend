using Sets.Api.Interfaces.Repositories;
using Sets.Api.Models.Entities;
using Shared.Models.Models;

namespace Sets.Tests.Repos;

public class MockedSetsRepo : ISetsRepository
{
    private List<Set> Sets { get; set; } = new();
    public async Task<Result<Set>> CreateSetAsync(Set set)
    {
        try
        {
            Sets.Add(set);
            
            return await Task.FromResult(Result.Success(set));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error<Set>(e.Message));
        }
    }

    public async Task<Result<Set>> PatchSetAsync(Set set)
    {
        try
        {
            var prevSet = Sets.Find(set1 => set1.Id == set.Id)!;
            
            Sets.Remove(prevSet);
            Sets.Add(set);
            
            return await Task.FromResult(Result.Success(set));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error<Set>(e.Message));
        }
    }

    public async Task<Result> RemoveSetAsync(Guid setId)
    {
        try
        {
            var setToRemove = Sets.Find(set => set.Id == setId)!;
            Sets.Remove(setToRemove);
            
            return await Task.FromResult(Result.Success());
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error(e.Message));
        }
    }

    public async Task<Result<Set>> GetSetAsync(Guid setId)
    {
        try
        {
            return await Task.FromResult(Result.Success(Sets.Find(set => set.Id == setId)!));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error<Set>(e.Message));
        }
    }

    public async Task<Result<List<Set>>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize)
    {
        try
        {
            return await Task.FromResult(Result.Success(Sets.FindAll(set => set.UserId == userId)));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error<List<Set>>(e.Message));
        }
    }

    public async Task<Result<List<Set>>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize, string searchQuery)
    {
        try
        {
            var allSetsQuery = Sets
                .Where(set => set.IsPublic)
                .Where(set => set.UserId != userId)
                .OrderBy(set => set.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            
            if (searchQuery != string.Empty) 
                allSetsQuery = allSetsQuery.Where(set => set.Name.Contains(searchQuery));
            
            return await Task.FromResult(Result.Success(allSetsQuery.ToList()));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error<List<Set>>(e.Message));
        }
    }

    public async Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic)
    {
        try
        {
            var userSets = Sets.Where(set => set.UserId == userId).ToList();
            
            Sets.RemoveAll(set => set.UserId == userId);
            userSets.ForEach(set => set.IsPublic = arePublic);
            Sets.AddRange(userSets);
            
            return await Task.FromResult(Result.Success());
        }
        catch (Exception e)
        {
            return await Task.FromResult(Result.Error(e.Message));
        }
    }

    public async Task<int> GetAllSetsPagesCountAsync(Guid userId, int pageSize)
    {
        var count = Sets
            .Where(set => set.IsPublic)
            .Count(set => set.UserId != userId);
        
        return await Task.FromResult(count);
    }

    public async Task<int> GetUserSetsPagesCountAsync(Guid userId, int pageSize)
    {
        var count = Sets
            .Count(set => set.UserId == userId);
        
        return await Task.FromResult(count);
    }
}
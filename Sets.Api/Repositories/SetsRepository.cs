using Microsoft.EntityFrameworkCore;
using Serilog;
using Sets.Api.Interfaces.Repositories;
using Sets.Api.Models.DbContext;
using Sets.Api.Models.Entities;
using Shared.Models.Models;

namespace Sets.Api.Repositories;

public class SetsRepository : ISetsRepository
{
    private readonly ApplicationContext _dbContext;

    public SetsRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Set>> CreateSetAsync(Set set)
    {
        try
        {
            await _dbContext.Sets.AddAsync(set);
            await _dbContext.SaveChangesAsync();

            return Result.Success(set);
        }
        catch (Exception e)
        {
            Log.Error("An error occured while creating new set. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error<Set>(e.Message);
        }
    }

    public async Task<Result<Set>> PatchSetAsync(Set set)
    {
        try
        {
            var oldSet = await _dbContext.Sets
                .Where(set1 => set1.Id == set.Id)
                .Include(set1 => set1.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
                .Include(set1 => set1.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings)
                .FirstOrDefaultAsync();

            if (oldSet is null)
                return Result.Error<Set>("This set does not exist");

            oldSet = set;
            _dbContext.Sets.Update(oldSet);
            await _dbContext.SaveChangesAsync();

            return Result.Success(set);
        }
        catch (Exception e)
        {
            Log.Error("An error occured while modifying an existing set. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error<Set>(e.Message);
        }
    }

    public async Task<Result> RemoveSetAsync(Guid setId)
    {
        try
        {
            var set = await _dbContext.Sets.Where(set1 => set1.Id == setId).FirstOrDefaultAsync();

            if (set is null)
                return Result.Error("This set does not exist");

            _dbContext.Remove(set);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An error occured while removing set. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error(e.Message);
        }
    }

    public async Task<Result<Set>> GetSetAsync(Guid setId)
    {
        try
        {
            var set = await _dbContext.Sets
                .Where(set1 => set1.Id == setId)
                .Include(set1 => set1.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
                .Include(set1 => set1.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings)
                .FirstOrDefaultAsync();

            return set is null ? Result.Error<Set>("No such set is present") : Result.Success(set);
        }
        catch (Exception e)
        {
            Log.Error("An error occured while getting set. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error<Set>(e.Message);
        }
    }

    public async Task<Result<List<Set>>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize)
    {
        try
        {
            var userSets = await _dbContext.Sets
                .Where(set => set.UserId == userId)
                .OrderBy(set => set.Name)
                .Include(set => set.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
                .Include(set => set.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result.Success(userSets);
        }
        catch (Exception e)
        {
            Log.Error("An error occured while querying user sets. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error<List<Set>>(e.Message);
        }
    }

    public async Task<Result<List<Set>>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize, string searchQuery)
    {
        try
        {
            var allSetsQuery = _dbContext.Sets
                .Where(set => set.IsPublic)
                .Where(set => set.UserId != userId)
                .OrderBy(set => set.Name)
                .Include(set => set.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
                .Include(set => set.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            if (searchQuery != string.Empty)
                allSetsQuery = allSetsQuery.Where(set => set.Name.Contains(searchQuery));

            return Result.Success(await allSetsQuery.ToListAsync());
        }
        catch (Exception e)
        {
            Log.Error("An error occured while querying all sets. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error<List<Set>>(e.Message);
        }
    }

    public async Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic)
    {
        try
        {
            var userSets = await _dbContext.Sets.Where(set => set.UserId == userId).ToListAsync();
            userSets.ForEach(set => set.IsPublic = arePublic);

            _dbContext.Sets.UpdateRange(userSets);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An error occured while querying all sets. Exception : {Exception}, Message: {Message}",
                e.GetType().FullName,
                e.Message);

            return Result.Error(e.Message);
        }
    }

    public async Task<int> GetAllSetsPagesCountAsync(Guid userId, int pageSize)
    {
        var count = await _dbContext.Sets
            .Where(set => set.IsPublic)
            .Where(set => set.UserId != userId)
            .CountAsync() / pageSize + 1;

        return count;
    }

    public async Task<int> GetUserSetsPagesCountAsync(Guid userId, int pageSize)
    {
        var count = await _dbContext.Sets
            .Where(set => set.UserId == userId)
            .CountAsync() / pageSize + 1;

        return count;
    }
}
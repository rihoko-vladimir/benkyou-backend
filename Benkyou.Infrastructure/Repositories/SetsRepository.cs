using AutoMapper;
using Benkyou.Application.Repositories;
using Benkyou.Domain.Database;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Benkyou.Infrastructure.Repositories;

public class SetsRepository : ISetsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public SetsRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<SetResponse>> CreateSetAsync(CreateSetRequest setRequest, Guid userId)
    {
        var user = await _dbContext.Users.Include(user => user.Cards).FirstOrDefaultAsync(user => user.Id == userId);
        if (user == null) return Result.Error<SetResponse>(new UserNotFoundException("Invalid Guid"));
        if (setRequest.KanjiList.Count < 3)
            return Result.Error<SetResponse>(new KanjiCountException("There must be at least 3 kanji at the card"));
        var card = new Set
        {
            Id = Guid.NewGuid(),
            Name = setRequest.SetName,
            Description = setRequest.SetDescription,
            KanjiList = setRequest.KanjiList,
            User = user,
            UserId = userId
        };
        foreach (var kanji in setRequest.KanjiList)
        {
            kanji.CardId = card.Id;
            kanji.Card = card;
            foreach (var kanjiKunyomiReading in kanji.KunyomiReadings)
            {
                kanjiKunyomiReading.Kanji = kanji;
                kanjiKunyomiReading.KanjiId = kanji.Id;
            }

            foreach (var kanjiOnyomiReading in kanji.OnyomiReadings)
            {
                kanjiOnyomiReading.Kanji = kanji;
                kanjiOnyomiReading.KanjiId = kanji.Id;
            }
        }

        await _dbContext.Sets.AddAsync(card);
        return Result.Success(_mapper.Map<SetResponse>(card));
    }

    public async Task<Result> ModifySetAsync(ModifySetRequest modifyRequest, Guid userId)
    {
        var user = await _dbContext.Users.Include(user => user.Cards).FirstOrDefaultAsync(user => user.Id == userId);
        if (user == null) return Result.Error(new UserNotFoundException("Invalid Guid"));
        if (modifyRequest.KanjiList.Count < 3)
            return Result.Error(new KanjiCountException("There must be at least 3 kanji at the card"));
        var card = user.Cards?.FirstOrDefault(card => card.Id == Guid.Parse(modifyRequest.SetId));
        if (card == null) return Result.Error(new SetUpdateException("Card with specified id wasn't found"));
        card.Name = modifyRequest.SetName;
        card.Description = modifyRequest.SetDescription;
        var kanjiListMapped = _mapper.Map<List<Kanji>>(modifyRequest.KanjiList);
        var kanjiToRemove = await _dbContext.KanjiList.Where(kanji => kanji.CardId == card.Id).ToListAsync();
        _dbContext.KanjiList.RemoveRange(kanjiToRemove);
        card.KanjiList = kanjiListMapped;
        _dbContext.Sets.Update(card);
        return Result.Success();
    }

    public async Task<Result> RemoveSetAsync(Guid cardId, Guid userId)
    {
        var card = await _dbContext.Sets.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) return Result.Error(new InvalidSetIdException("Card with specified id wasn't found"));
        if (card.UserId != userId)
            return Result.Error(new CardRemoveException("You can't remove cards of other users"));
        _dbContext.Sets.Remove(card);
        return Result.Success();
    }

    public async Task<Result<List<SetResponse>>> GetUserSetsAsync(Guid userId)
    {
        var cards = await _dbContext.Sets.Where(card => card.UserId == userId)
            .OrderBy(set => set.Name)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings).ToListAsync();
        return cards.Count == 0
            ? Result.Success(new List<SetResponse>())
            : Result.Success(_mapper.Map<List<SetResponse>>(cards));
    }

    public async Task<Result<int>> GetAllSetsPageCountAsync(Guid userId, int pageSize)
    {
        var sets = _dbContext.Sets
            .Where(set => set.User.IsAccountPublic)
            .Where(set => set.UserId != userId);
        decimal count = await sets.CountAsync();
        var pageCount = (int) Math.Ceiling(count / pageSize);
        return Result.Success(pageCount);
    }

    public async Task<Result<int>> GetAllSetsByQueryPageCountAsync(Guid userId, int pageNumber, int pageSize,
        string searchQuery)
    {
        var sets = _dbContext.Sets
            .Where(set => set.User.IsAccountPublic)
            .Where(set => set.UserId != userId)
            .Where(set => set.Name.Contains(searchQuery));
        decimal count = await sets.CountAsync();
        try
        {
            var pageCount = (int) Math.Ceiling(count / pageSize);
            return Result.Success(pageCount);
        }
        catch (Exception)
        {
            return Result.Error<int>(new Exception("Something went wrong"));
        }
    }

    public async Task<Result<List<SetResponse>>> GetAllSetsByPageAsync(Guid userId, int pageNumber, int pageSize)
    {
        var sets = _dbContext.Sets
            .Where(set => set.User.IsAccountPublic)
            .OrderBy(set => set.Id)
            .Where(set => set.UserId != userId)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings);
        var count = await sets.CountAsync();
        if (count == 0) return Result.Success(new List<SetResponse>());
        if (count <= pageSize) return Result.Success(_mapper.Map<List<SetResponse>>(await sets.ToListAsync()));
        var setsToReturn =
            _mapper.Map<List<SetResponse>>(await sets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync());
        return Result.Success(setsToReturn);
    }

    public async Task<Result<List<SetResponse>>> GetAllSetsAsync()
    {
        var sets = await _dbContext.Sets
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings)
            .ToListAsync();
        var setsToReturn =
            _mapper.Map<List<SetResponse>>(sets);
        return Result.Success(setsToReturn);
    }

    public async Task<Result> ReportSetAsync(Guid userId, Guid setId, string reportReason)
    {
        var set = await _dbContext.Sets.Where(set1 => set1.Id == setId).ToListAsync();
        var isReportingOwnSet = set[0].UserId == userId;
        if (isReportingOwnSet)
        {
            return Result.Error(new Exception("You can not report your own sets"));
        }
        var report = new Report
        {
            SetId = setId,
            UserId = userId,
            Description = reportReason
        };
        await _dbContext.Reports.AddAsync(report);
        return Result.Success();
    }

    public async Task<Result<List<SetResponse>>> GetSetsByQueryAsync(Guid userId, string searchQuery, int pageNumber,
        int pageSize)
    {
        var sets = _dbContext.Sets
            .Where(set => set.User.IsAccountPublic)
            .OrderBy(set => set.Id)
            .Where(set => set.Name.Contains(searchQuery))
            .Where(set => set.UserId != userId)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings);
        var count = await sets.CountAsync();
        if (count == 0) return Result.Success(new List<SetResponse>());
        if (count <= pageSize) return Result.Success(_mapper.Map<List<SetResponse>>(await sets.ToListAsync()));
        var setsToReturn =
            _mapper.Map<List<SetResponse>>(await sets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync());
        return Result.Success(setsToReturn);
    }


    public async Task<Result<SetResponse>> GetSetAsync(Guid cardId)
    {
        var card = await _dbContext.Sets.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) Result.Error(new InvalidSetIdException("Card with specified id wasn't found"));
        return Result.Success(_mapper.Map<SetResponse>(card));
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
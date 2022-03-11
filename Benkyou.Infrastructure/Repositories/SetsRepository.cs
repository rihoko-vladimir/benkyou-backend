﻿using AutoMapper;
using Benkyou.Application.Repositories;
using Benkyou.Domain.Database;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
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

    public async Task<Result<Guid>> CreateSetAsync(CreateSetRequest setRequest, Guid userId)
    {
        var user = await _dbContext.Users.Include(user => user.Cards).FirstOrDefaultAsync(user => user.Id == userId);
        if (user == null) return Result.Error<Guid>(new UserNotFoundExceptions("Invalid Guid"));
        if (setRequest.KanjiList.Count < 3)
            return Result.Error<Guid>(new KanjiCountException("There must be at least 3 kanji at the card"));
        var card = new Card
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

        await _dbContext.Cards.AddAsync(card);
        return Result.Success(card.Id);
    }

    public async Task<Result> ModifySetNameAsync(Guid cardId, string newName, Guid userId)
    {
        var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) return Result.Error(new InvalidCardIdException("Card with specified id wasn't found"));
        if (card.UserId != userId)
            return Result.Error(new CardUpdateException("You can't change set name of other users"));
        card.Name = newName;
        _dbContext.Update(card);
        return Result.Success();
    }

    public async Task<Result> ModifySetDescriptionAsync(Guid cardId, string newDescription, Guid userId)
    {
        var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) return Result.Error(new InvalidCardIdException("Card with specified id wasn't found"));
        if (card.UserId != userId)
            return Result.Error(new CardUpdateException("You can't change set description of other users"));
        card.Description = newDescription;
        _dbContext.Update(card);
        return Result.Success();
    }

    public async Task<Result> ModifySetKanjiListAsync(Guid cardId, List<KanjiRequest> kanjiList, Guid userId)
    {
        var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) return Result.Error(new InvalidCardIdException("Card with specified id wasn't found"));
        if (card.UserId != userId)
            return Result.Error(new CardUpdateException("You can't change set kanji of other users"));
        var kanjiListMapped = _mapper.Map<List<Kanji>>(kanjiList);
        var kanjiToRemove = await _dbContext.KanjiList.Where(kanji => kanji.CardId == card.Id).ToListAsync();
        _dbContext.KanjiList.RemoveRange(kanjiToRemove);
        card.KanjiList = kanjiListMapped;
        _dbContext.Update(card);
        return Result.Success();
    }

    public async Task<Result> RemoveSetAsync(Guid cardId, Guid userId)
    {
        var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) return Result.Error(new InvalidCardIdException("Card with specified id wasn't found"));
        if (card.UserId != userId)
            return Result.Error(new CardRemoveException("You can't remove cards of other users"));
        _dbContext.Cards.Remove(card);
        return Result.Success();
    }

    public async Task<Result<List<CardResponse>>> GetAllSetsAsync(Guid userId)
    {
        var cards = await _dbContext.Cards.Where(card => card.UserId == userId)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.KunyomiReadings)
            .Include(card => card.KanjiList).ThenInclude(kanji => kanji.OnyomiReadings).ToListAsync();
        return cards.Count == 0
            ? Result.Success(new List<CardResponse>())
            : Result.Success(_mapper.Map<List<CardResponse>>(cards));
    }

    public async Task<Result<CardResponse>> GetSetAsync(Guid cardId)
    {
        var card = await _dbContext.Cards.FirstOrDefaultAsync(card => card.Id == cardId);
        if (card == null) Result.Error(new InvalidCardIdException("Card with specified id wasn't found"));
        return Result.Success(_mapper.Map<CardResponse>(card));
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
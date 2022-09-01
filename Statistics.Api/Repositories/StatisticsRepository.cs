using MongoDB.Driver;
using Serilog;
using Shared.Models.Models;
using Statistics.Api.Interfaces.Repositories;
using Statistics.Api.Models.Configurations;
using Statistics.Api.Models.Entities;

namespace Statistics.Api.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly MongoClient _client;

    public StatisticsRepository(MongoConfig mongoConfig)
    {
        _client = new MongoClient(mongoConfig.ConnectionString);
    }

    public async Task<Result> SetLastOnlineStatus(Guid userId, DateTime lastOnlineDateTime)
    {
        try
        {
            await _client.GetDatabase("benkyou_statistics")
                .GetCollection<GeneralStatistics>("generalStatistics")
                .UpdateOneAsync(statistics => statistics.UserId == userId,
                    Builders<GeneralStatistics>.Update.Set(statistics => statistics.LastTimeOnline,
                        lastOnlineDateTime));
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while setting up new online status, Exception: {ExceptionType}, Stacktrace: {StackTrace}",
                e.GetType().FullName, e.StackTrace);
            return Result.Error();
        }
    }

    public async Task SetRegistrationDate(Guid userId, DateTime registrationDateTime)
    {
        try
        {
            await _client.GetDatabase("benkyou_statistics")
                .GetCollection<GeneralStatistics>("generalStatistics")
                .InsertOneAsync(new GeneralStatistics()
                {
                    UserId = userId,
                    LastTimeOnline = DateTime.Now,
                    RegistrationDateTime = registrationDateTime
                });
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while setting up registration date and time, Exception: {ExceptionType}, Stacktrace: {StackTrace}",
                e.GetType().FullName, e.StackTrace);
        }
    }

    public async Task<Result> AddSetLearnResult(Guid userId, Guid setId, DateTime startDate, DateTime endDate, StudyResult[] studyResult)
    {
        try
        {
            await _client.GetDatabase("benkyou_statistics")
                .GetCollection<StudyResultStatistics>("studyResultStatistics")
                .InsertOneAsync(new StudyResultStatistics
                {
                    UserId = userId,
                    SetId = setId,
                    StudyResults = studyResult,
                });
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while adding set learn result, Exception: {ExceptionType}, Stacktrace: {StackTrace}",
                e.GetType().FullName, e.StackTrace);

            return Result.Error();
        }
    }

    public async Task<Result<List<StudyResultStatistics>>> GetSetStudyResults(Guid userId, Guid setId)
    {
        try
        {
            return Result.Success(await (await _client.GetDatabase("benkyou_statistics")
                .GetCollection<StudyResultStatistics>("studyResultStatistics")
                .FindAsync(statistics => statistics.UserId == userId && statistics.SetId == setId)).ToListAsync());
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while adding set learn result, Exception: {ExceptionType}, Stacktrace: {StackTrace}",
                e.GetType().FullName, e.StackTrace);

            return Result.Error<List<StudyResultStatistics>>("An error occured while attempting to get set results");
        }
    }

    public async Task<Result<GeneralStatistics>> GetGeneralStatistics(Guid userId)
    {
        try
        {
            return Result.Success(await (await _client.GetDatabase("benkyou_statistics")
                .GetCollection<GeneralStatistics>("generalStatistics")
                .FindAsync(statistics => true)).FirstAsync());
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while getting general statistics, Exception: {ExceptionType}, Stacktrace: {StackTrace}",
                e.GetType().FullName, e.StackTrace);

            return Result.Error<GeneralStatistics>("An error occured while attempting to get general user statistics");
        }
    }
}
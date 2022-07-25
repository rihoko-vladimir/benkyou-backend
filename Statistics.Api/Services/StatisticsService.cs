using Serilog;
using Shared.Models.Models;
using Statistics.Api.Interfaces.Repositories;
using Statistics.Api.Interfaces.Services;
using Statistics.Api.Models.Entities;

namespace Statistics.Api.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result> SetLastOnlineStatus(Guid userId)
    {
        var currentDateTime = DateTime.Now;

        Log.Information("Setting last online status {LastOnline} for user {UserId}", currentDateTime, userId);

        return await _statisticsRepository.SetLastOnlineStatus(userId, currentDateTime);
    }

    public async Task SetRegistrationDate(Guid userId, DateTime registrationDateTime)
    {
        Log.Information("Setting registration date {RegistrationDate} for user {UserId}", registrationDateTime, userId);

        await _statisticsRepository.SetRegistrationDate(userId, registrationDateTime);
    }

    public async Task<Result> AddSetLearnResult(Guid userId, Guid setId, DateTime startDate, DateTime endDate,
        StudyResult[] studyResults)
    {
        Log.Information("Adding set learn result {StudyResult} for set {SetId} for user {UserId}",
            studyResults.ToString(), setId, userId);

        return await _statisticsRepository.AddSetLearnResult(userId, setId, startDate, endDate, studyResults);
    }

    public async Task<Result<List<StudyResultStatistics>>> GetSetStudyResults(Guid userId, Guid setId)
    {
        Log.Information("Getting set study results for user {UserId} for set {SetId}", userId, setId);

        return await _statisticsRepository.GetSetStudyResults(userId, setId);
    }

    public async Task<Result<GeneralStatistics>> GetGeneralStatistics(Guid userId)
    {
        Log.Information("Getting general statistics for user {UserId}", userId);

        return await _statisticsRepository.GetGeneralStatistics(userId);
    }
}
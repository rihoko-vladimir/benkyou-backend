using Shared.Models.Models;
using Statistics.Api.Models.Entities;

namespace Statistics.Api.Interfaces.Services;

public interface IStatisticsService
{
    public Task<Result> SetLastOnlineStatus(Guid userId);

    public Task SetRegistrationDate(Guid userId, DateTime registrationDateTime);

    public Task<Result> AddSetLearnResult(Guid userId, Guid setId, DateTime startDate, DateTime endDate, StudyResult[] studyResults);

    public Task<Result<List<StudyResultStatistics>>> GetSetStudyResults(Guid userId, Guid setId);
    
    public Task<Result<GeneralStatistics>> GetGeneralStatistics(Guid userId);
}
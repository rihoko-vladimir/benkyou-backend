using Shared.Models.Models;

namespace Sets.Api.Interfaces.Services;

public interface ISenderService
{
    public Task<Result> SendLearnFinishResultMessage(Guid userId, Guid setId, DateTime startDateTime, DateTime endDateTime,
        KanjiResult[] kanjiResults);
}
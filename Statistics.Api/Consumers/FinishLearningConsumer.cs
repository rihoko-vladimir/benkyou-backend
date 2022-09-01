using AutoMapper;
using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Statistics.Api.Interfaces.Services;
using Statistics.Api.Models.Entities;

namespace Statistics.Api.Consumers;

public class FinishLearningConsumer : IConsumer<FinishLearningMessage>
{
    private readonly IStatisticsService _statisticsService;
    private readonly IMapper _mapper;

    public FinishLearningConsumer(IStatisticsService statisticsService, IMapper mapper)
    {
        _statisticsService = statisticsService;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<FinishLearningMessage> context)
    {
        try
        {
            var message = context.Message;

            Log.Information("Received set results message for set {SetId}", message.SetId);

            var studyResults = _mapper.Map<StudyResult[]>(message.KanjiResults);
            
            await _statisticsService.AddSetLearnResult(message.UserId, message.SetId, message.StartDateTime, message.EndDateTime, studyResults);
        }
        catch (Exception)
        {
            Log.Error("An error occured while processing registration message for user {UserId}",
                context.Message.UserId);
        }
    }
}
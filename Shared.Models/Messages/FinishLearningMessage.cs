using Shared.Models.Models;

namespace Shared.Models.Messages;

public class FinishLearningMessage
{
    public Guid UserId { get; init; }
    public Guid SetId { get; init; }

    public DateTime StartDateTime { get; init; }

    public DateTime EndDateTime { get; init; }

    public KanjiResult[] KanjiResults { get; init; }
}
using Shared.Models.Models;

namespace Sets.Api.Models.Requests;

public class FinishLearningRequest
{
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public KanjiResult[] KanjiResults { get; set; }
}
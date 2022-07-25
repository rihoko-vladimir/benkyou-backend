using Shared.Models.Models;

namespace Shared.Models.Messages;

public class FinishLearningMessage
{
    public Guid UserId { get; set; }
    public Guid SetId { get; set; }
    
    public DateTime StartDateTime { get; set; }
    
    public DateTime EndDateTime { get; set; }
    
    public KanjiResult[] KanjiResults { get; set; }
    
    
}
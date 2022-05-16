namespace Benkyou.Domain.Models.Requests;

public class ReportRequest
{
    public string SetId { get; set; }
    public string UserId { get; set; }
    public string ReportReason { get; set; }
}
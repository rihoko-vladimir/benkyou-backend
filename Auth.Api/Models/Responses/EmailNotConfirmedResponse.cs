namespace Auth.Api.Models.Responses;

public class EmailNotConfirmedResponse
{
    public Guid Id { get; set; }
    
    public string Message { get; set; }

    public EmailNotConfirmedResponse(Guid id, string message)
    {
        Id = id;
        Message = message;
    }
}
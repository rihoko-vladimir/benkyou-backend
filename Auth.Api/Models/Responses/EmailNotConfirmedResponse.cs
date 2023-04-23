namespace Auth.Api.Models.Responses;

public class EmailNotConfirmedResponse
{
    public EmailNotConfirmedResponse(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
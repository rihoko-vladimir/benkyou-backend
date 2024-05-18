namespace Sets.Api.Models.Responses;

public class SetResponse
{
    public Guid Id { get; init; }
    public Guid AuthorId { get; init; }

    public string Name { get; init; }

    public string Description { get; init; }

    public ICollection<KanjiResponse> KanjiList { get; init; }
}
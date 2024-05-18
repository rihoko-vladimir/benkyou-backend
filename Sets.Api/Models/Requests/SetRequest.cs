namespace Sets.Api.Models.Requests;

public class SetRequest
{
    public string Name { get; init; }

    public string Description { get; init; }

    public ICollection<KanjiRequest> KanjiList { get; init; }
}
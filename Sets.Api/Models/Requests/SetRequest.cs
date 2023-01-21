namespace Sets.Api.Models.Requests;

public class SetRequest
{
    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<KanjiRequest> KanjiList { get; set; }
}
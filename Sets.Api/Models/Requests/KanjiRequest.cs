namespace Sets.Api.Models.Requests;

public class KanjiRequest
{
    public string KanjiChar { get; init; }

    public ICollection<KunyomiRequest> KunyomiReadings { get; init; }

    public ICollection<OnyomiRequest> OnyomiReadings { get; init; }
}
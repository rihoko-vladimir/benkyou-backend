namespace Sets.Api.Models.Requests;

public class KanjiRequest
{
    public string KanjiChar { get; set; }

    public ICollection<KunyomiRequest> KunyomiReadings { get; set; }

    public ICollection<OnyomiRequest> OnyomiReadings { get; set; }
}
using System.Text.Json.Serialization;

namespace Sets.Api.Models.Responses;

public class KanjiResponse
{
    [JsonPropertyName("kanji")] 
    public string KanjiChar { get; set; }

    [JsonPropertyName("kunyomiReadings")] 
    public ICollection<KunyomiResponse> KunyomiReadings { get; set; }

    [JsonPropertyName("onyomiReadings")] 
    public ICollection<OnyomiResponse> OnyomiReadings { get; set; }
}
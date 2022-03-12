using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Responses;

public class KanjiResponse
{
    [JsonPropertyName("kanji")] public string KanjiChar { get; set; }

    [JsonPropertyName("kunyoumiReadings")] public ICollection<KunyomiResponse> KunyomiReadings { get; set; }

    [JsonPropertyName("onyoumiReadings")] public ICollection<OnyomiResponse> OnyomiReadings { get; set; }
}
using System.Text.Json.Serialization;

namespace Sets.Api.Models.Responses;

public class KanjiResponse
{
    [JsonPropertyName("kanji")] public string KanjiChar { get; init; }

    [JsonPropertyName("kunyomiReadings")] public ICollection<KunyomiResponse> KunyomiReadings { get; init; }

    [JsonPropertyName("onyomiReadings")] public ICollection<OnyomiResponse> OnyomiReadings { get; init; }
}
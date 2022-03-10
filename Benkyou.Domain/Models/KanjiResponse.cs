using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models;

public class KanjiResponse
{
    [JsonPropertyName("kanji")] public string KanjiChar { get; set; }

    [JsonPropertyName("kunyoumiReadings")] public ICollection<KunyomiResult> KunyomiReadings { get; set; }

    [JsonPropertyName("onyoumiReadings")] public ICollection<OnyomiResult> OnyomiReadings { get; set; }
}
using MongoDB.Bson.Serialization.Attributes;

namespace Statistics.Api.Models.Entities;

public class StudyResult
{
    [BsonElement("kanji")] public char Kanji { get; set; }

    [BsonElement("selectedKunyomi")] public string[] SelectedKunyomi { get; set; }

    [BsonElement("selectedOnyomi")] public string[] SelectedOnyomi { get; set; }

    [BsonElement("correctKunyomi")] public string[] CorrectKunyomi { get; set; }

    [BsonElement("correctOnyomi")] public string[] CorrectOnyomi { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(Kanji)}: {Kanji}, {nameof(SelectedKunyomi)}: {SelectedKunyomi}, {nameof(SelectedOnyomi)}: {SelectedOnyomi}, {nameof(CorrectKunyomi)}: {CorrectKunyomi}, {nameof(CorrectOnyomi)}: {CorrectOnyomi}";
    }
}
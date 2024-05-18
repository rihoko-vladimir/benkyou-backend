using MongoDB.Bson.Serialization.Attributes;

namespace Statistics.Api.Models.Entities;

public class StudyResult
{
    [BsonElement("kanji")] public char Kanji { get; init; }

    [BsonElement("selectedKunyomi")] public string[] SelectedKunyomi { get; init; }

    [BsonElement("selectedOnyomi")] public string[] SelectedOnyomi { get; init; }

    [BsonElement("correctKunyomi")] public string[] CorrectKunyomi { get; init; }

    [BsonElement("correctOnyomi")] public string[] CorrectOnyomi { get; init; }

    public override string ToString()
    {
        return
            $"{nameof(Kanji)}: {Kanji}, {nameof(SelectedKunyomi)}: {SelectedKunyomi}, {nameof(SelectedOnyomi)}: {SelectedOnyomi}, {nameof(CorrectKunyomi)}: {CorrectKunyomi}, {nameof(CorrectOnyomi)}: {CorrectOnyomi}";
    }

    protected bool Equals(StudyResult other)
    {
        return Kanji == other.Kanji && SelectedKunyomi.SequenceEqual(other.SelectedKunyomi) &&
               SelectedOnyomi.SequenceEqual(other.SelectedOnyomi) &&
               CorrectKunyomi.SequenceEqual(other.CorrectKunyomi) && CorrectOnyomi.SequenceEqual(other.CorrectOnyomi);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StudyResult)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Kanji, SelectedKunyomi, SelectedOnyomi, CorrectKunyomi, CorrectOnyomi);
    }
}
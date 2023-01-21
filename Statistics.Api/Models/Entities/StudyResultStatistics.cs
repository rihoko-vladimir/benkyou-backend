using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Statistics.Api.Models.Entities;

public class StudyResultStatistics
{
    [BsonId] public ObjectId Id { get; set; }

    [BsonElement("setId")] public Guid SetId { get; set; }

    [BsonElement("userId")] public Guid UserId { get; set; }

    [BsonElement("studyResults")] public StudyResult[] StudyResults { get; set; }

    [BsonElement("startDate")] public DateTime StartDate { get; set; }

    [BsonElement("endDate")] public DateTime EndDate { get; set; }

    protected bool Equals(StudyResultStatistics other)
    {
        return SetId.Equals(other.SetId) && UserId.Equals(other.UserId) && StudyResults.Equals(other.StudyResults) &&
               StartDate.ToString("yyyyMMddHHmmss") == other.StartDate.ToString("yyyyMMddHHmmss") &&
               EndDate.ToString("yyyyMMddHHmmss") == other.EndDate.ToString("yyyyMMddHHmmss");
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StudyResultStatistics)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SetId, UserId, StudyResults, StartDate, EndDate);
    }
}
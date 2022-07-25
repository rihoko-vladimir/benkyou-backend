using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Statistics.Api.Models.Entities;

public class StudyResultStatistics
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("setId")]
    public Guid SetId { get; set; }
    
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("studyResults")]
    public StudyResult[] StudyResults { get; set; }

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }
    
    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }
}
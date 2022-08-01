using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Statistics.Api.Models.Entities;

public class GeneralStatistics
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("registrationDateTime")]
    public DateTime RegistrationDateTime { get; set; }
    
    [BsonElement("lastTimeOnline")]
    public DateTime LastTimeOnline { get; set; }
}
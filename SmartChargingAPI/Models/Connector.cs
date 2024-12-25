using MongoDB.Bson.Serialization.Attributes;

namespace SmartChargingAPI.Models;

public class Connector : IEntity
{
    [BsonId]
    public string Id { get; set; }
    public int MaxCurrentAmps { get; set; }
}
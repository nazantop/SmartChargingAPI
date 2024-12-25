using MongoDB.Bson.Serialization.Attributes;

namespace SmartChargingAPI.Models;

public class Connector : IEntity
{
    [BsonId]
    public string Id { get; set; }
    public int MaxCurrentAmps { get; set; }
    public Connector(string id, int maxCurrentAmps)
    {
        if (maxCurrentAmps <= 0) throw new ArgumentException("Max current must be greater than zero.");

        Id = id;
        MaxCurrentAmps = maxCurrentAmps;
    }
}
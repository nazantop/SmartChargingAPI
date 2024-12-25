using MongoDB.Bson.Serialization.Attributes;

namespace SmartChargingAPI.Models;

public class ChargeStation : IEntity
{
    [BsonId]
    public string Id { get; set;} = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public List<Connector> Connectors { get; set; } = new();
    public ChargeStation(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        Name = name;
    }

    public int CapacityInAmps => Connectors?.Sum(c => c.MaxCurrentAmps) ?? 0;
}
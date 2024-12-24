using MongoDB.Bson.Serialization.Attributes;

namespace SmartChargingAPI.Models;

public class Group: IEntity
{
    [BsonId]
    public string Id { get; set;} = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public int CapacityAmps { get; set; }
    public List<ChargeStation> ChargeStations { get; set;} = [];

    public Group(string name, int capacityAmps)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (capacityAmps <= 0) throw new ArgumentException("Capacity must be greater than zero.");

        Name = name;
        CapacityAmps = capacityAmps;
    }
    
    public int CapacityInAmps => ChargeStations?.Sum(cs => cs.CapacityInAmps) ?? 0;

}
using MongoDB.Bson.Serialization.Attributes;

namespace SmartChargingAPI.Models;

public class Group: IEntity
{
    [BsonId]
    public string Id { get; set;} = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public int? CapacityAmps { get; set; }
    public List<ChargeStation> ChargeStations { get; set;} = [];
    public int CapacityInAmps => ChargeStations?.Sum(cs => cs.CapacityInAmps) ?? 0;

}
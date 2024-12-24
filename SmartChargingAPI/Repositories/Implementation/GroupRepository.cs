using MongoDB.Driver;
using SmartChargingAPI.Models;
using SmartChargingAPI.Repositories.Interfaces;

namespace SmartChargingAPI.Repositories.Implementation;

public class GroupRepository : IGroupRepository
{
    private readonly IMongoCollection<Group> _groupCollection;

    public GroupRepository(IMongoClient mongoClient, IConfiguration configuration)
    {
        var database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _groupCollection = database.GetCollection<Group>(configuration["MongoDB:Containers:Groups"]);
    }

    public async Task<Group> AddAsync(Group group)
    {
        await _groupCollection.InsertOneAsync(group);
        return group;
    }

    public async Task<bool> RemoveAsync(Guid groupId)
    {
        var result = await _groupCollection.DeleteOneAsync(g => g.Id == groupId.ToString());
        return result.DeletedCount > 0;
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _groupCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Group?> GetByIdAsync(Guid groupId)
    {
        return await _groupCollection.Find(g => g.Id == groupId.ToString()).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(Guid groupId, UpdateDefinition<Group> updateDefinition)
    {
        var result = await _groupCollection.UpdateOneAsync(g => g.Id == groupId.ToString(), updateDefinition);
        return result.ModifiedCount > 0;
    }

    public async Task<Group?> GetGroupByStationIdAsync(Guid stationId)
    {
        return await _groupCollection
            .Find(g => g.ChargeStations.Any(cs => cs.Id == stationId.ToString()))
            .FirstOrDefaultAsync();
    }
}
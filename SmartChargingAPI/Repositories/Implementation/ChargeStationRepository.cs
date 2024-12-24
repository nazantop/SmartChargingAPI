using MongoDB.Driver;
using SmartChargingAPI.Models;
using SmartChargingAPI.Repositories.Interfaces;

namespace SmartChargingAPI.Repositories.Implementation
{
    public class ChargeStationRepository : IChargeStationRepository
    {
        private readonly IMongoCollection<Group> _groupCollection;

        public ChargeStationRepository(IMongoClient mongoClient, IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _groupCollection = database.GetCollection<Group>(configuration["MongoDB:Containers:Groups"]);
        }

        public async Task<ChargeStation?> GetByIdAsync(Guid stationId)
        {
            var group = await _groupCollection
                .Find(g => g.ChargeStations.Any(cs => cs.Id == stationId.ToString()))
                .FirstOrDefaultAsync();

            return group?.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());
        }

        public async Task<bool> UpdateChargeStationsAsync(Guid groupId, List<ChargeStation> updatedStations)
        {
            var updateDefinition = Builders<Group>.Update.Set(g => g.ChargeStations, updatedStations);
            var result = await _groupCollection.UpdateOneAsync(g => g.Id == groupId.ToString(), updateDefinition);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddStationAsync(Guid groupId, ChargeStation station)
        {
            var group = await _groupCollection.Find(g => g.Id == groupId.ToString()).FirstOrDefaultAsync();
            if (group == null) return false;

            group.ChargeStations.Add(station);

            var updateDefinition = Builders<Group>.Update.Set(g => g.ChargeStations, group.ChargeStations);
            var result = await _groupCollection.UpdateOneAsync(g => g.Id == groupId.ToString(), updateDefinition);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveStationAsync(Guid groupId, Guid stationId)
        {
            var group = await _groupCollection.Find(g => g.Id == groupId.ToString()).FirstOrDefaultAsync();
            if (group == null) return false;

            var station = group.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());
            if (station == null) return false;

            group.ChargeStations.Remove(station);

            var updateDefinition = Builders<Group>.Update.Set(g => g.ChargeStations, group.ChargeStations);
            var result = await _groupCollection.UpdateOneAsync(g => g.Id == groupId.ToString(), updateDefinition);

            return result.ModifiedCount > 0;
        }
    }
}

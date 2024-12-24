using MongoDB.Driver;
using SmartChargingAPI.Models;

namespace SmartChargingAPI.Repositories.Interfaces;
public interface IGroupRepository
{
    Task<Group> AddAsync(Group group);
    Task<bool> RemoveAsync(Guid groupId);
    Task<List<Group>> GetAllAsync();
    Task<Group?> GetByIdAsync(Guid groupId);
    Task<bool> UpdateAsync(Guid groupId, UpdateDefinition<Group> updateDefinition);
    Task<Group?> GetGroupByStationIdAsync(Guid stationId);
}

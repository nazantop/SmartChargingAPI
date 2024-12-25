using SmartChargingAPI.Models;

namespace SmartChargingAPI.Repositories.Interfaces
{
    public interface IChargeStationRepository
    {
        Task<ChargeStation?> GetByIdAsync(Guid stationId);
        Task<ChargeStation?> UpdateChargeStationsAsync(Guid groupId, List<ChargeStation> updatedStations);
        Task<bool> AddStationAsync(Guid groupId, ChargeStation station);
        Task<bool> RemoveStationAsync(Guid groupId, Guid stationId);
    }
}

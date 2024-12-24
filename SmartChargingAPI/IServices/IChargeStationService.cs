using SmartChargingAPI.Models;
using SmartChargingAPI.Models.Common;

namespace SmartChargingAPI.IServices;
public interface IChargeStationService
{
    Task<ValidationResult<ChargeStation>> AddChargeStation(Guid groupId, ChargeStation station);
    Task<ValidationResult<bool>> RemoveChargeStation(Guid stationId);
    Task<ValidationResult<bool>> UpdateChargeStation(Guid stationId, ChargeStation station);
    Task<ValidationResult<ChargeStation>> GetChargeStationById(Guid id);
}
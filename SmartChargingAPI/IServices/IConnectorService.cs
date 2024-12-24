using SmartChargingAPI.Models;
using SmartChargingAPI.Models.Common;

namespace SmartChargingAPI.IServices;
public interface IConnectorService
{
    Task<ValidationResult<Connector?>> AddConnector(Guid stationId, Connector connector);
    Task<ValidationResult<bool>> UpdateConnector(Guid stationId, Connector updatedConnector);
    Task<ValidationResult<bool>> RemoveConnector(Guid stationId, int connectorId);
}
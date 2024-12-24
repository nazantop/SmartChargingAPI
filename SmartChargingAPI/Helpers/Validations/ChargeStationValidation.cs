using SmartChargingAPI.Models;

namespace SmartChargingAPI.Helpers.Validations;

public static class ChargeStationValidation
{
     public static string? ValidateStation(ChargeStation? station, Guid stationId, ILogger logger)
    {
        if (station == null)
        {
            var errorMessage = $"Station {stationId} not found.";
            logger.LogWarning(errorMessage);
            return errorMessage;
        }

        return null;
    }
    public static string? ValidateGroupByGivenStation(Group? group, Guid stationId, ILogger logger)
    {
        if (group == null)
        {
            var errorMessage = $"Station {stationId} not found in a group.";
            logger.LogWarning(errorMessage);
            return errorMessage;
        }

        return null;
    }
    
}
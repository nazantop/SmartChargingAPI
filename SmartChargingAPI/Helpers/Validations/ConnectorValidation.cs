using SmartChargingAPI.Models;

namespace SmartChargingAPI.Helpers.Validations
{
    public static class ConnectorValidation
    {

        public static string? ValidateConnectorLimit(ChargeStation station, Guid stationId, ILogger logger)
        {
            if (station.Connectors.Count >= 5)
            {
                var errorMessage = $"Station {stationId} already has the maximum number of connectors.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }

       public static string? ValidateGroupCapacity(Group? group, ChargeStation? station, ILogger logger)
        {
            if(group == null || station == null){
                var errorMessage =  "Group or station couldn't found.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }
            var totalCapacity = group.ChargeStations
                .SelectMany(cs => cs.Connectors)
                .Sum(c => c.MaxCurrentAmps);

            if (totalCapacity > group.CapacityAmps)
            {
                var errorMessage = $"Adding connector exceeds group capacity for station {station.Id}. Group capacity: {group.CapacityAmps}, Total after addition: {totalCapacity}. Rolling back.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }

        public static string? ValidateConnectorExists(ChargeStation? station, string connectorId, ILogger logger)
        {
            if (station == null || station.Connectors.All(c => c.Id != connectorId))
            {
                var errorMessage = $"Connector {connectorId} not found in station {station.Id}.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }
    }
}
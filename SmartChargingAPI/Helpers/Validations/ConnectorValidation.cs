using SmartChargingAPI.Models;

namespace SmartChargingAPI.Helpers.Validations
{
    public static class ConnectorValidation
    {
        public static string? ValidateGroupCapacity(Group? group, ChargeStation? station, ILogger logger)
        {
            if (group == null || station == null)
            {
                var errorMessage = "Group or station couldn't be found.";
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
                var errorMessage = $"Connector {connectorId} not found in station {station?.Id}.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }

        public static string? ValidateConnectorOnRemove(ChargeStation? station, ILogger logger)
        {
             if (station?.Connectors.Count <= 1){ 
                logger.LogWarning("Cannot remove the last connector from station {StationId}. Delete the charge station instead.", station.Id);
                var errorMessage = "Cannot remove the last connector from a charge station. Delete the charge station instead.";
                return errorMessage;
                }

            return null;
        }

        public static string? ValidateConnectorCount(List<Connector>? connectors, ILogger logger)
        {
            if (connectors == null || connectors.Count < 1 || connectors.Count > 5)
            {
                var errorMessage = "A charge station must have between 1 and 5 connectors.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }

        public static string? ValidateConnectorMaxCurrent(List<Connector> connectors, ILogger logger)
        {
            if (connectors.Any(x=> x.MaxCurrentAmps <= 0 || x.MaxCurrentAmps ==null))
            {
                var errorMessage = $"Connector max current must be greater than zero.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }

        public static string? ValidateAddingConnectorExceedsCapacity(int totalGroupCapacity, int? maxCurrentAmps, int? groupCapacity, ILogger logger)
        {
            if (totalGroupCapacity + maxCurrentAmps > groupCapacity)
            {
                var errorMessage = $"Adding connector exceeds group capacity. Group capacity: {groupCapacity}, Total after addition: {totalGroupCapacity + maxCurrentAmps}.";
                logger.LogWarning(errorMessage);
                return errorMessage;
            }

            return null;
        }
    }
}
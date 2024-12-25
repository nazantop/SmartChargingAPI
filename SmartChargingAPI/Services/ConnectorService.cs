using System.Collections.Concurrent;
using SmartChargingAPI.Helpers.Validations;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Models;
using SmartChargingAPI.Models.Common;
using SmartChargingAPI.Repositories.Interfaces;

namespace SmartChargingAPI.Services
{
    public class ConnectorService : IConnectorService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IChargeStationRepository _chargeStationRepository;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreDictionary = new();
        private readonly ILogger<ConnectorService> _logger;

        public ConnectorService(
            IGroupRepository groupRepository,
            IChargeStationRepository chargeStationRepository,
            ILogger<ConnectorService> logger)
        {
            _groupRepository = groupRepository;
            _chargeStationRepository = chargeStationRepository;
            _logger = logger;
            _logger.LogInformation("ConnectorService initialized.");
        }

        private string GetSemaphoreKey(Guid stationId) => $"{stationId}";

        private SemaphoreSlim GetOrCreateSemaphore(Guid stationId)
        {
            return _semaphoreDictionary.GetOrAdd(GetSemaphoreKey(stationId), _ => new SemaphoreSlim(1, 1));
        }

        public async Task<ValidationResult<Connector>> AddConnector(Guid stationId, Connector connector)
        {
            _logger.LogInformation("Attempting to add connector to station {StationId} with max current {MaxCurrentAmps}.", stationId, connector.MaxCurrentAmps);

            var group = await _groupRepository.GetGroupByStationIdAsync(stationId);
            var validationMessage = ChargeStationValidation.ValidateGroupByGivenStation(group, stationId, _logger);
            if (validationMessage != null) return ValidationResult<Connector>.Failure(validationMessage);

            var station = group?.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());

            validationMessage = ConnectorValidation.ValidateConnectorLimit(station, stationId, _logger);
            if (validationMessage != null) return ValidationResult<Connector>.Failure(validationMessage);

            var semaphore = GetOrCreateSemaphore(stationId);
            await semaphore.WaitAsync();
            try
            {
                var existingIds = station.Connectors.Select(c => c.Id).ToHashSet();
                var nextId = Enumerable.Range(1, 5).First(id => !existingIds.Contains(id.ToString()));

                var newConnector = new Connector(nextId.ToString(), connector.MaxCurrentAmps);
                station.Connectors.Add(newConnector);

                validationMessage = ConnectorValidation.ValidateGroupCapacity(group, station, _logger);
                if (validationMessage != null)
                {
                    station.Connectors.Remove(newConnector);
                    return ValidationResult<Connector>.Failure(validationMessage);
                }

                var updateChargeStations = await _chargeStationRepository.UpdateChargeStationsAsync(Guid.Parse(group.Id), group.ChargeStations);
                if (updateChargeStations != null)
                {
                    _logger.LogInformation("Connector added successfully to station {StationId}.", stationId);
                    return ValidationResult<Connector>.Success(newConnector);
                }

                return ValidationResult<Connector>.Failure("Failed to update group when adding connector.");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ValidationResult<bool>> UpdateConnector(Guid stationId, Connector updatedConnector)
        {
            _logger.LogInformation("Attempting to update connector {ConnectorId} in station {StationId}.", updatedConnector.Id, stationId);

            var group = await _groupRepository.GetGroupByStationIdAsync(stationId);
            var validationMessage = ChargeStationValidation.ValidateGroupByGivenStation(group, stationId, _logger);
            if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

            var station = group?.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());

            var connector = station?.Connectors.FirstOrDefault(c => c.Id == updatedConnector.Id);
            validationMessage = ConnectorValidation.ValidateConnectorExists(station, updatedConnector.Id, _logger);
            if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

            var semaphore = GetOrCreateSemaphore(stationId);
            await semaphore.WaitAsync();
            try
            {
                connector.MaxCurrentAmps = updatedConnector.MaxCurrentAmps;

                validationMessage = ConnectorValidation.ValidateGroupCapacity(group, station, _logger);
                if (validationMessage != null)
                {
                    connector.MaxCurrentAmps = 0; // Revert in case of validation failure
                    return ValidationResult<bool>.Failure(validationMessage);
                }

                var updateChargeStations = await _chargeStationRepository.UpdateChargeStationsAsync(Guid.Parse(group.Id), group.ChargeStations);
                if (updateChargeStations != null)
                {
                    _logger.LogInformation("Connector updated successfully.");
                    return ValidationResult<bool>.Success(true);
                }

                return ValidationResult<bool>.Failure("Failed to update connector.");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ValidationResult<bool>> RemoveConnector(Guid stationId, int connectorId)
        {
            _logger.LogInformation("Attempting to remove connector {ConnectorId} from station {StationId}.", connectorId, stationId);

            var group = await _groupRepository.GetGroupByStationIdAsync(stationId);
            var validationMessage = ChargeStationValidation.ValidateGroupByGivenStation(group, stationId, _logger);
            if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

            var station = group?.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());

            validationMessage = ConnectorValidation.ValidateConnectorExists(station, connectorId.ToString(), _logger);
            if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

            var semaphore = GetOrCreateSemaphore(stationId);
            await semaphore.WaitAsync();
            try
            {
                station.Connectors.RemoveAll(c => c.Id == connectorId.ToString());

                var updateChargeStations = await _chargeStationRepository.UpdateChargeStationsAsync(Guid.Parse(group.Id), group.ChargeStations);
                if (updateChargeStations != null)
                {
                    _logger.LogInformation("Connector removed successfully.");
                    return ValidationResult<bool>.Success(true);
                }

                return ValidationResult<bool>.Failure("Failed to remove connector.");
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
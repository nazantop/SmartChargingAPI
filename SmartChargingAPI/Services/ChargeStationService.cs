using SmartChargingAPI.Models;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Repositories.Interfaces;
using SmartChargingAPI.Models.Common;
using SmartChargingAPI.Helpers.Validations;

namespace SmartChargingAPI.Services;

public class ChargeStationService : IChargeStationService
{
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupService _groupService;
    private readonly ILogger<ChargeStationService> _logger;

    public ChargeStationService(IGroupRepository groupRepository,IChargeStationRepository chargeStationRepository, IGroupService groupService, ILogger<ChargeStationService> logger)
    {
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
        _groupService = groupService;
        _logger = logger;
        _logger.LogInformation("ChargeStationService initialized.");
    }

    public async Task<ValidationResult<ChargeStation>> AddChargeStation(Guid groupId, ChargeStation chargeStation)
    {
        _logger.LogInformation("Attempting to add charge station to group {GroupId} with connectors.", groupId);

        var group = await _groupRepository.GetByIdAsync(groupId);
        var validationMessage = GroupValidation.ValidateGroup(group, _logger);
        if (validationMessage != null) return ValidationResult<ChargeStation>.Failure(validationMessage);

        validationMessage = ConnectorValidation.ValidateConnectorCount(chargeStation.Connectors, _logger);
        if (validationMessage != null) return ValidationResult<ChargeStation>.Failure(validationMessage);

        var newStation = new ChargeStation(chargeStation.Name);
        var totalGroupCapacity = group?.ChargeStations
            .SelectMany(cs => cs.Connectors)
            .Sum(c => c.MaxCurrentAmps) ?? 0;

        foreach (var connectorRequest in chargeStation.Connectors)
        {
            validationMessage = ConnectorValidation.ValidateConnectorMaxCurrent(connectorRequest, _logger);
            if (validationMessage != null) return ValidationResult<ChargeStation>.Failure(validationMessage);

            validationMessage = ConnectorValidation.ValidateAddingConnectorExceedsCapacity(
                totalGroupCapacity,
                connectorRequest.MaxCurrentAmps,
                group.CapacityAmps,
                _logger);
            if (validationMessage != null) return ValidationResult<ChargeStation>.Failure(validationMessage);

            var connectorId = (newStation.Connectors.Count + 1).ToString();
            newStation.Connectors.Add(new Connector(connectorId, connectorRequest.MaxCurrentAmps));
            totalGroupCapacity += connectorRequest.MaxCurrentAmps;
        }

        group.ChargeStations.Add(newStation);

        var updateChargeStations = await _chargeStationRepository.UpdateChargeStationsAsync(groupId, group.ChargeStations);
        if (updateChargeStations != null)
        {
            _logger.LogInformation("Charge station with connectors added successfully to group {GroupId}.", groupId);
            return ValidationResult<ChargeStation>.Success(newStation);
        }

        return ValidationResult<ChargeStation>.Failure("Failed to update group when adding charge station.");
    }



    public async Task<ValidationResult<bool>> RemoveChargeStation(Guid stationId)
    {
        _logger.LogInformation("Attempting to remove charge station {StationId}.", stationId);

        var group = await _groupRepository.GetGroupByStationIdAsync(stationId);
        var validationMessage = ChargeStationValidation.ValidateGroupByGivenStation(group, stationId, _logger);
        if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

        var updateSuccess = await _chargeStationRepository.RemoveStationAsync(Guid.Parse(group.Id), stationId);

        if (updateSuccess)
        {
            _logger.LogInformation("Charge station {StationId} removed successfully.", stationId);
            return ValidationResult<bool>.Success(true);
        }

        return ValidationResult<bool>.Failure("Failed to update group when removing charge station.");
    }

    public async Task<ValidationResult<bool>> UpdateChargeStation(Guid stationId, ChargeStation updatedStation)
    {
        _logger.LogInformation("Attempting to update charge station {StationId} to name {NewName}.", stationId, updatedStation.Name);

        var group = await _groupRepository.GetGroupByStationIdAsync(stationId);
        var validationMessage = ChargeStationValidation.ValidateGroupByGivenStation(group, stationId, _logger);
        if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

        var station = group.ChargeStations.FirstOrDefault(cs => cs.Id == stationId.ToString());
        
        station.Name = updatedStation.Name;

        var updateChargeStations = await _chargeStationRepository.UpdateChargeStationsAsync(Guid.Parse(group.Id), group.ChargeStations);

        if (updateChargeStations != null)
        {
            _logger.LogInformation("Charge station {StationId} updated successfully to name {NewName}.", stationId, updatedStation.Name);
            return ValidationResult<bool>.Success(true);
        }

        return ValidationResult<bool>.Failure("Failed to update charge station.");
    }

    public async Task<ValidationResult<ChargeStation>> GetChargeStationById(Guid stationId)
    {
        _logger.LogInformation("Attempting to retrieve charge station with ID {StationId}.", stationId);

        var station = await _chargeStationRepository.GetByIdAsync(stationId);
        var validationMessage = ChargeStationValidation.ValidateStation(station, stationId, _logger);
        if (validationMessage != null) return ValidationResult<ChargeStation>.Failure(validationMessage);

        _logger.LogInformation("Charge station {StationId} retrieved successfully.", stationId);
        return ValidationResult<ChargeStation>.Success(station);
    }
}
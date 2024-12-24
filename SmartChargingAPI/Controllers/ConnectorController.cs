using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SmartChargingAPI.DTOs;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Models;

[ApiController]
[Route("api/stations/{stationId}/connectors")]
public class ConnectorController : ControllerBase
{
    private readonly IConnectorService _connectorService;
    private readonly IMapper _mapper;
    private readonly ILogger<ConnectorController> _logger;

    public ConnectorController(
        IConnectorService connectorService,
        IMapper mapper,
        ILogger<ConnectorController> logger)
    {
        _connectorService = connectorService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddConnector(Guid stationId, [FromBody] ConnectorRequestDto connectorRequestDto)
    {
        _logger.LogInformation("Adding connector to station {StationId} with max current: {MaxCurrentAmps}", stationId, connectorRequestDto.MaxCurrentAmps);

         var connectorRequest = _mapper.Map<Connector>(connectorRequestDto);

        var createdConnector = await _connectorService.AddConnector(stationId, connectorRequest);
        if (createdConnector.IsSuccess)
        {
            _logger.LogInformation("Successfully added connector {ConnectorId} to station {StationId}", createdConnector?.Data?.Id, stationId);

            var response = _mapper.Map<ConnectorResponseDto>(createdConnector?.Data);
            return Ok(response);
        }

        _logger.LogWarning(createdConnector.Message, stationId);
        return BadRequest(createdConnector.Message);
    }

    [HttpPut("{connectorId}")]
    public async Task<IActionResult> UpdateConnector(Guid stationId, int connectorId, [FromBody] ConnectorRequestDto updatedConnectorDto)
    {
        _logger.LogInformation("Updating connector {ConnectorId} for station {StationId} with max current: {MaxCurrentAmps}", connectorId, stationId, updatedConnectorDto.MaxCurrentAmps);

        var updatedConnector = _mapper.Map<Connector>(updatedConnectorDto);
        updatedConnector.Id = connectorId.ToString();

        var connectorResult = await _connectorService.UpdateConnector(stationId, updatedConnector);
        if (connectorResult.IsSuccess)
        {
            _logger.LogInformation("Successfully updated connector {ConnectorId} for station {StationId}", connectorId, stationId);

            var response = _mapper.Map<ConnectorResponseDto>(updatedConnector);
            return Ok(response);
        }

        _logger.LogWarning(connectorResult.Message, connectorId, stationId);
        return BadRequest(connectorResult.Message);
    }

    [HttpDelete("{connectorId}")]
    public async Task<IActionResult> RemoveConnector(Guid stationId, int connectorId)
    {
        _logger.LogInformation("Removing connector {ConnectorId} from station {StationId}", connectorId, stationId);

        var connectorResult = await _connectorService.RemoveConnector(stationId, connectorId);
        if (connectorResult.IsSuccess)
        {
            _logger.LogInformation("Successfully removed connector {ConnectorId} from station {StationId}", connectorId, stationId);
            return NoContent();
        }

        _logger.LogWarning(connectorResult.Message, connectorId, stationId);
        return NotFound(connectorResult.Message);
    }
}
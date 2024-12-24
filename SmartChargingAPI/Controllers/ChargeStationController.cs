using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SmartChargingAPI.DTOs;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Models;

[ApiController]
[Route("api/charge-stations")]
public class ChargeStationController : ControllerBase
{
    private readonly IChargeStationService _chargeStationService;
    private readonly IMapper _mapper;
    private readonly ILogger<ChargeStationController> _logger;

    public ChargeStationController(
        IChargeStationService chargeStationService, 
        IMapper mapper,
        ILogger<ChargeStationController> logger)
    {
        _chargeStationService = chargeStationService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("{groupId}")]
    public async Task<IActionResult> AddStation(Guid groupId, [FromBody] ChargeStationRequestDto stationRequestDto)
    {
        _logger.LogInformation("Adding charge station to group {GroupId} with request: {@StationRequestDto}", groupId, stationRequestDto);

        var stationRequest = _mapper.Map<ChargeStation>(stationRequestDto);

        var stationResult = await _chargeStationService.AddChargeStation(groupId, stationRequest);
        if (stationResult.IsSuccess)
        {
            _logger.LogInformation("Successfully added charge station {StationId} to group {GroupId}", stationRequest.Id, groupId);

            var response = _mapper.Map<ChargeStationResponseDto>(stationRequest);
            return Ok(response);
        }

        _logger.LogWarning(stationResult.Message, groupId);
        return BadRequest(stationResult.Message);
    }

    [HttpDelete("{stationId}")]
    public async Task<IActionResult> RemoveStation(Guid stationId)
    {
        _logger.LogInformation("Removing charge station {StationId}", stationId);

        var stationResult = await _chargeStationService.RemoveChargeStation(stationId);
        if (stationResult.IsSuccess)
        {
            _logger.LogInformation("Successfully removed charge station {StationId}", stationId);

            return NoContent();
        }

        _logger.LogWarning(stationResult.Message, stationId);
        return NotFound(stationResult.Message);
    }

    [HttpPut("{stationId}")]
    public async Task<IActionResult> UpdateStation(Guid stationId, [FromBody] ChargeStationRequestDto updatedStationDto)
    {
        _logger.LogInformation("Updating charge station {StationId} with request: {@UpdatedStationDto}", stationId, updatedStationDto);

        var updatedStation = _mapper.Map<ChargeStation>(updatedStationDto);

        var stationResult = await _chargeStationService.UpdateChargeStation(stationId, updatedStation);
        if (stationResult.IsSuccess)
        {
            _logger.LogInformation("Successfully updated charge station {StationId}", stationId);
            return Ok();
        }

        _logger.LogWarning(stationResult.Message, stationId);
        return NotFound(stationResult.Message);
    }
    [HttpGet("{stationId}")]
    public async Task<IActionResult> GetChargeStationById(Guid stationId)
    {
        _logger.LogInformation("Received request to retrieve charge station with ID {StationId}.", stationId);

        var result = await _chargeStationService.GetChargeStationById(stationId);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved charge station with ID {StationId}.", stationId);
             return Ok(result.Data);
          
        }

        _logger.LogWarning(result.Message, stationId);
        return NotFound(result.Message);
        
    }
}
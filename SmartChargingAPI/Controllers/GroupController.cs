using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SmartChargingAPI.DTOs;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Models;

[ApiController]
[Route("api/groups")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IMapper _mapper;
    private readonly ILogger<GroupController> _logger;

    public GroupController(IGroupService groupService, IMapper mapper, ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] List<GroupRequestDto> groupRequestDto)
    {
        _logger.LogInformation("Attempting to create a group.");

        var groupRequest = _mapper.Map<List<Group>>(groupRequestDto);

        var createdGroup = await _groupService.AddGroup(groupRequest);
        if (createdGroup.IsSuccess)
        {
            _logger.LogInformation("Groups created successfully");

            var response = _mapper.Map<List<GroupResponseDto>>(createdGroup?.Data);
            return Ok(response);
        }

        _logger.LogWarning(createdGroup.Message);
        return BadRequest(createdGroup.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        _logger.LogInformation("Retrieving all groups.");

        var groups = await _groupService.GetGroups();
        _logger.LogInformation("{Count} groups retrieved.", groups?.Data?.Count);

        var response = _mapper.Map<List<GroupResponseDto>>(groups?.Data);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveGroup(Guid id)
    {
        _logger.LogInformation("Attempting to remove group with ID: {GroupId}", id);

        var removeGroup = await _groupService.RemoveGroup(id);
        if (removeGroup.IsSuccess)
        {
            _logger.LogInformation("Group with ID: {GroupId} removed successfully.", id);
            return NoContent();
        }

        _logger.LogWarning(removeGroup.Message, id);
        return NotFound(removeGroup.Message);
    }

    [HttpPut("{groupId}")]
    public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] GroupRequestDto updatedGroupDto)
    {
        _logger.LogInformation("Attempting to update group with ID: {GroupId}, New Name: {Name}, New Capacity: {CapacityAmps}", groupId, updatedGroupDto.Name, updatedGroupDto.CapacityAmps);

        var groupRequest = _mapper.Map<Group>(updatedGroupDto);
        var updatedGroup = await _groupService.UpdateGroup(groupId, groupRequest);

        if (updatedGroup.IsSuccess)
        {
            _logger.LogInformation("Group with ID: {GroupId} updated successfully.", groupId);
            return Ok();
        }

        _logger.LogWarning(updatedGroup.Message, groupId);
        return BadRequest(updatedGroup.Message);
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetGroupById(Guid groupId)
    {
        _logger.LogInformation("Received request to retrieve group with ID {GroupId}.", groupId);

        var result = await _groupService.GetGroupById(groupId);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved group with ID {GroupId}.", groupId);
            return Ok(result.Data);
           
        }
        _logger.LogWarning("Failed to retrieve group with ID {GroupId}. Reason: {Reason}.", groupId, result.Message);
        return NotFound(result.Message);
    }
}
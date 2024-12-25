using SmartChargingAPI.Models;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Repositories.Interfaces;
using SmartChargingAPI.Models.Common;
using MongoDB.Driver;
using SmartChargingAPI.Helpers.Validations;

namespace SmartChargingAPI.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _logger = logger;
        _logger.LogInformation("GroupService initialized.");
    }

   public async Task<ValidationResult<List<Group>>> AddGroup(List<Group> newGroups)
{
    _logger.LogInformation("Attempting to add a list of groups.");

    var addedGroups = new List<Group>();

    foreach (var newGroup in newGroups)
    {
        _logger.LogInformation("Adding a new group with name {Name} and capacity {CapacityAmps}.", newGroup.Name, newGroup.CapacityAmps);

        var validationMessage = GroupValidation.ValidateGroupCreation(newGroup.Name, newGroup.CapacityAmps, _logger);
        if (validationMessage != null)
        {
            _logger.LogWarning("Validation failed for group {Name}: {ValidationMessage}", newGroup.Name, validationMessage);
            return ValidationResult<List<Group>>.Failure($"Validation failed for group {newGroup.Name}: {validationMessage}");
        }

        var group = new Group{Name = newGroup.Name, CapacityAmps = newGroup.CapacityAmps, ChargeStations = []};

        var addedGroup = await _groupRepository.AddAsync(group);
        _logger.LogInformation("Group added successfully with ID {GroupId}.", addedGroup.Id);

        addedGroups.Add(addedGroup);
    }

    return ValidationResult<List<Group>>.Success(addedGroups);
}


    public async Task<ValidationResult<bool>> RemoveGroup(Guid groupId)
    {
        _logger.LogInformation("Attempting to remove group with ID {GroupId}.", groupId);

        var group = await _groupRepository.GetByIdAsync(groupId);
        var validationMessage = GroupValidation.ValidateGroup(group, _logger);
        if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

        var result = await _groupRepository.RemoveAsync(groupId);
        if (result)
        {
            _logger.LogInformation("Group with ID {GroupId} removed successfully.", groupId);
            return ValidationResult<bool>.Success(true);
        }

        return ValidationResult<bool>.Failure("Failed to remove group. It may not exist.");
    }

    public async Task<ValidationResult<List<Group>>> GetGroups()
    {
        _logger.LogInformation("Retrieving all groups.");
        var groups = await _groupRepository.GetAllAsync();

        foreach (var group in groups)
        {
            group.ChargeStations ??= new List<ChargeStation>();
        }

        _logger.LogInformation("Retrieved {GroupCount} groups.", groups.Count);
        return ValidationResult<List<Group>>.Success(groups);
    }

    public async Task<ValidationResult<Group>> GetGroupById(Guid groupId)
    {
        _logger.LogInformation("Retrieving group with ID {GroupId}.", groupId);

        var group = await _groupRepository.GetByIdAsync(groupId);
       var validationMessage = GroupValidation.ValidateGroup(group, _logger);
        if (validationMessage != null) return ValidationResult<Group>.Failure(validationMessage);

        _logger.LogInformation("Group with ID {GroupId} retrieved successfully.", groupId);
        return ValidationResult<Group>.Success(group);
    }

    public async Task<ValidationResult<bool>> UpdateGroup(Guid groupId, Group updatedGroup)
    {
        _logger.LogInformation("Attempting to update group with ID {GroupId}.", groupId);

        var group = await GetGroupById(groupId);

        if (updatedGroup.CapacityAmps == null) updatedGroup.CapacityAmps = group.Data.CapacityAmps; 
        var validationMessage = GroupValidation.ValidateGroupForUpdate(group.Data, updatedGroup.CapacityAmps, _logger);
        if (validationMessage != null) return ValidationResult<bool>.Failure(validationMessage);

        if (string.IsNullOrWhiteSpace(updatedGroup.Name)) updatedGroup.Name = group.Data.Name; 

        var updateDefinition = Builders<Group>.Update
            .Set(g => g.Name, updatedGroup.Name)
            .Set(g => g.CapacityAmps, updatedGroup.CapacityAmps);

        var result = await _groupRepository.UpdateAsync(groupId, updateDefinition);
        if (result)
        {
            _logger.LogInformation("Group with ID {GroupId} updated successfully.", groupId);
            return ValidationResult<bool>.Success(true);
        }

        return ValidationResult<bool>.Failure("Failed to update group.");
    }
}
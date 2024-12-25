using SmartChargingAPI.Models;

namespace SmartChargingAPI.Helpers.Validations;

public static class GroupValidation
{
    public static string? ValidateGroupCreation(string name, int? capacityAmps, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogWarning("Group name is invalid.");
            return "Group name is required.";
        }

        if (capacityAmps <= 0 || capacityAmps == null)
        {
            logger.LogWarning("Group capacity {CapacityAmps} is invalid.", capacityAmps);
            return "Group capacity must be greater than zero.";
        }

        return null;
    }

    public static string? ValidateGroup(Group? group, ILogger logger)
    {
        if (group == null)
        {
            logger.LogWarning("Group not found.");
            return "Group not found.";
        }

        return null;
    }

    public static string? ValidateGroupForUpdate(Group? group, int? newCapacity, ILogger logger)
    {
        if (group == null)
        {
            logger.LogWarning("Group not found for update.");
            return "Group not found.";
        }

        if (newCapacity < group.CapacityInAmps)
        {
            logger.LogWarning("New capacity {NewCapacity} is less than the current usage {CurrentUsage}.", newCapacity, group.CapacityInAmps);
            return "New capacity is less than the current usage.";
        }

        return null;
    }
}
using SmartChargingAPI.Models;
using SmartChargingAPI.Models.Common;

namespace SmartChargingAPI.IServices;
public interface IGroupService
{
    Task<ValidationResult<List<Group>>> AddGroup(List<Group> group);
    Task<ValidationResult<bool>> RemoveGroup(Guid groupId);
    Task<ValidationResult<List<Group>>> GetGroups();
    Task<ValidationResult<Group>> GetGroupById(Guid groupId);
    Task<ValidationResult<bool>> UpdateGroup(Guid groupId, Group group);
}
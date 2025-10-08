using Logic.Dtos;

namespace Logic.Interfaces;

public interface IUserRoleManager
{
    Task<IEnumerable<RoleLogic>> GetRolesForUserAsync(Guid userId);
    Task AddUserRoleAsync(UserRoleLogic userRole);
    Task DeleteUserRoleAsync(UserRoleLogic userRole);
}
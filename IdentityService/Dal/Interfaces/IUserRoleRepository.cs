using Dal.Models;

namespace Dal.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<RoleDal>> GetRolesForUserAsync(Guid userId);
    Task AddUserRoleAsync(UserRoleDal userRole);
    Task DeleteUserRoleAsync(Guid userId, Guid roleId);
}
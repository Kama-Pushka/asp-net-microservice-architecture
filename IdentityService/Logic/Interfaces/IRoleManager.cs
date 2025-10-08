using Logic.Dtos;

namespace Logic.Interfaces;

public interface IRoleManager
{
    Task<RoleLogic> GetRoleByIdAsync(Guid id);
    Task<IEnumerable<RoleLogic>> GetAllRolesAsync();
    Task AddRoleAsync(RoleLogic role);
    Task UpdateRoleAsync(RoleLogic role);
    Task DeleteRoleAsync(Guid id);
}
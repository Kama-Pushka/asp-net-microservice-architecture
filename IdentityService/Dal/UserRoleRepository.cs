using Dal.Interfaces;
using Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly SqliteDbContext _context;

    public UserRoleRepository(SqliteDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDal>> GetRolesForUserAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .Join(
                _context.Roles,
                userRole => userRole,
                role => role.Id,
                (userRole, role) => role
            )
            .ToListAsync();
    }

    public async Task AddUserRoleAsync(UserRoleDal userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserRoleAsync(Guid userId, Guid roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }
}
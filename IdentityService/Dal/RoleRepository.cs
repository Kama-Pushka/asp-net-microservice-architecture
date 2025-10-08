using Dal.Interfaces;
using Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class RoleRepository : IRoleRepository
{
    private readonly SqliteDbContext _context;

    public RoleRepository(SqliteDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDal> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<IEnumerable<RoleDal>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task AddAsync(RoleDal role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoleDal role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
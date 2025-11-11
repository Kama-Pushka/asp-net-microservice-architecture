using System.Linq.Expressions;
using Dal.Interfaces;
using Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Dal;

public class UserRepository : IUserRepository
{
    private readonly SqliteDbContext _context;

    public UserRepository(SqliteDbContext context)
    {
        _context = context;
    }

    public async Task<UserDal> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<UserDal>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(UserDal user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserDal user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser != null)
        {
            if (!string.IsNullOrEmpty(user.Username)) existingUser.Username = user.Username;
            if (!string.IsNullOrEmpty(user.Email)) existingUser.Email = user.Email;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
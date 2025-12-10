using Microsoft.EntityFrameworkCore;

namespace lab1.Dal;

public interface IIpRepository
{
    Task Add(IpDal dal);
    Task<List<IpDal>> GetAll();
}

public class IpRepository : IIpRepository
{
    private readonly IpContext _context;

    public IpRepository(IpContext context)
    {
        _context = context;
    }

    public async Task Add(IpDal dal)
    {
        await _context.IpRecords.AddAsync(dal);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IpDal>> GetAll()
    {
        return await _context.IpRecords.ToListAsync();
    }
}
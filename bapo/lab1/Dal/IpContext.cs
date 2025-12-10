using Microsoft.EntityFrameworkCore;

namespace lab1.Dal;

public class IpContext : DbContext
{
    public IpContext(DbContextOptions<IpContext> options) : base(options) {}
    
    public DbSet<IpDal> IpRecords { get; set; }
}
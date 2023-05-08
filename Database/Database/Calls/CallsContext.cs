using Database.Database.Calls.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Database.Calls;

public class CallsContext : DbContext
{
    public DbSet<CallHeader> Headers { get; set; }
    public DbSet<Call> Calls { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AvailableHeader> AvailableHeaders { get; set; }


    public CallsContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("partman");
    }
}
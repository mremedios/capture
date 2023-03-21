using System;
using Capture.Service.Database.Calls.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Capture.Service.Database.Calls;

public class CallsContext : DbContext
{
    public DbSet<Header> Headers { get; set; }
    public DbSet<Call> Calls { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AvailableHeader> AvailableHeaders { get; set; }


    public CallsContext(DbContextOptions options) : base(options)
    {
    }
}
using System;
using Capture.Service.Database.Calls.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Capture.Service.Database.Calls;

public class CallsContext : DbContext
{
    public DbSet<Header> Headers { get; set; }
    public DbSet<Call> Calls { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AvailableHeader> AvailableHeaders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=localhost;Database=capture;");
        dataSourceBuilder.MapComposite<Header>("headertype");

        var dataSource = dataSourceBuilder.Build();

        optionsBuilder.UseNpgsql(dataSource)
            .LogTo(Console.WriteLine)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
using Npgsql;

namespace Capture.Service.Database.Calls;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Capture.Service.Database.Calls.Entities;

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
        
        optionsBuilder.UseNpgsql(dataSource); //todo 
        optionsBuilder.LogTo(Console.WriteLine);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        // NpgsqlConnection.GlobalTypeMapper.MapComposite<Header>("headertype"); // todo deprecated
    }
}


[Table("available_headers")]
public class AvailableHeader
{
    [Key, Column("header")] public string Header { get; set; }
}
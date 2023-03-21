using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Capture.Service.Database.JsonHeaders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database.JsonHeaders
{
    public class JsonContext : DbContext
    {
        public DbSet<Header> Headers { get; set; }
        public DbSet<AvailableHeader> AvailableHeaders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=capture-json;"); //todo 
            optionsBuilder.LogTo(Console.WriteLine);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // ето что
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Header>()
                .ToTable("sources")
                .Property(p => p.protocol_header)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }
    }

    public class Header
    {
        [Key] public DateTime created_date { get; set; }
        
         public string call_id { get; set; } // todo return key
        
        public string endpoint { get; set; }

        [Column(TypeName = "jsonb")] public Dictionary<string, string> protocol_header { get; set; }

        public string raw { get; set; }
    }

    [Table("available_headers")]
    public class AvailableHeader
    {
        [Key, Column("header")] 
        
        public string Header { get; set; }
    }
}
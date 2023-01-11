using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Capture.Service.Database
{
    public class JsonContext : DbContext
    {
        public DbSet<Header> Headers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseNpgsql("Host=localhost;Database=capture-json;"); //todo 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Header>()
                // .HasNoKey()
                .ToTable("sources")
                .Property(p => p.protocol_header)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }
    }

    public class Header
    {
        [Key]
        public DateTime created_date { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "jsonb")]
        public Dictionary<string, string> protocol_header { get; set; }
        public string raw { get; set; }
    }
}
using System;
using Capture.Service.Database.Calls;
using Capture.Service.Database.Calls.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Capture.Service.Database;

public class PostgreSqlContextFactory : IContextFactory
{
    private DataBaseConnectionConfig _config;

    public PostgreSqlContextFactory(IConfiguration config)
    {
        _config = config.GetSection("DataBase").Get<DataBaseConnectionConfig>();
    }

    public CallsContext CreateContext()
    {
        var connectionString =
            $"Host={_config.Address};Database={_config.Database};Maximum Pool Size={_config.MaxConnections}";

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (_config.Database == "capture")
        {
            var builder = new DbContextOptionsBuilder<CallsContext>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapComposite<Header>("header_type");
                
            var dataSource = dataSourceBuilder.Build();
            
            builder.UseNpgsql(dataSource)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .LogTo(Console.WriteLine, LogLevel.Debug);

            return new CallsContext(builder.Options);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
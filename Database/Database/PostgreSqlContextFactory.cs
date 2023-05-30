using Database.Database.Calls;
using Database.Database.Calls.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Database.Database;

public class PostgreSqlContextFactory : IContextFactory
{
    private DataBaseConnectionConfig _config;
    private NpgsqlDataSource _dataSource;

    public PostgreSqlContextFactory(IConfiguration config)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        _config = config.GetSection("DataBase").Get<DataBaseConnectionConfig>();
        var  connectionString = $"Host={_config.Address};Database={_config.Database};Username={_config.Username};Password={_config.Password};Maximum Pool Size={_config.MaxConnections}";
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapComposite<CallHeader>("header_type");
        _dataSource = dataSourceBuilder.Build();
    }

    public CallsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CallsContext>();
        
        options.UseNpgsql(_dataSource)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .LogTo(Console.WriteLine, LogLevel.Error);

        return new CallsContext(_config.Schema, options.Options);
    }
}


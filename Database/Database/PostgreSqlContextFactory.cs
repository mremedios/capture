using Database.Database.Calls;
using Database.Database.Calls.Models;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.NameTranslation;

namespace Database.Database;

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
            $"Host={_config.Address};Database={_config.Database};Username={_config.Username};Password={_config.Password};Maximum Pool Size={_config.MaxConnections}";

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = new DbContextOptionsBuilder<CallsContext>();
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapComposite<CallHeader>("header_type");
        
        var dataSource = dataSourceBuilder.Build();

        builder.UseNpgsql(dataSource)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .LogTo(Console.WriteLine, LogLevel.Error);
        builder.EnableServiceProviderCaching(false);
       
        return new CallsContext(builder.Options);
    }
}


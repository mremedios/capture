using Microsoft.Extensions.Configuration;

namespace Database.Database.Calls;

public class PartmanRepository : IPartmanRepository
{
    private readonly IContextFactory _contextFactory;
    private String _schema;

    public PartmanRepository(IContextFactory contextFactory, IConfiguration conf)
    {
        _contextFactory = contextFactory;
        _schema = conf.GetSection("Database").Get<DataBaseConnectionConfig>().Schema;
    }

    public async Task UpdateRetention(int days)
    {
        var daysStr = $"{days} days";
        using var ctx = _contextFactory.CreateContext();
        await ctx.StoredProcedure($"{_schema}.set_retention", daysStr);
    }
}
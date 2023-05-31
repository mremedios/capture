using Microsoft.Extensions.Configuration;

namespace Database.Database.Calls;

public class PartmanRepository : IPartmanRepository
{
    private readonly IContextFactory _contextFactory;

    public PartmanRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task UpdateRetention(int days)
    {
        var daysStr = $"{days} days";
        using var ctx = _contextFactory.CreateContext();
        await ctx.StoredProcedure("set_retention", daysStr);
    }
}
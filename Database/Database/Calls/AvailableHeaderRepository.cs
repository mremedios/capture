using Database.Database.Calls.Models;

namespace Database.Database.Calls;

public class AvailableHeaderRepository : IAvailableHeaderRepository
{

	private readonly IContextFactory _contextFactory;

	public AvailableHeaderRepository(IContextFactory contextFactory)
	{
		_contextFactory = contextFactory;
	}

	public async Task InsertAsync(string[] headers)
	{
		using var ctx = _contextFactory.CreateContext();
		await ctx.AvailableHeaders.AddRangeAsync(
			headers.Select(h => new AvailableHeader { Header = h })
		);
		await ctx.SaveChangesAsync();
	}

	public void Delete(string[] headers)
	{
		using var ctx = _contextFactory.CreateContext();
		ctx.AvailableHeaders.RemoveRange(
			headers.Select(h => new AvailableHeader { Header = h })
		);
		ctx.SaveChanges();
	}

	public string[] FindAll()
	{
		using var ctx = _contextFactory.CreateContext();
		return ctx.AvailableHeaders.Select(x => x.Header).ToArray();
	}
}
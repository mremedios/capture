using Database.Database.Calls.Models;
using Database.Models;

namespace Database.Database.Calls;

public class MethodsRepository : IMethodsRepository
{
	private readonly IContextFactory _contextFactory;

	public MethodsRepository(IContextFactory contextFactory)
	{
		_contextFactory = contextFactory;
	}

	public async Task InsertAsync(SipMethods[] methods)
	{
		using var ctx = _contextFactory.CreateContext();
		await ctx.Methods.AddRangeAsync(
			methods.Select(h => new Method { Value = h.ToString() })
		);
		ctx.SaveChanges();
	}

	public void Delete(SipMethods[] methods)
	{
		using var ctx = _contextFactory.CreateContext();
		ctx.Methods.RemoveRange(
			methods.Select(h => new Method { Value = h.ToString() })
		);
		ctx.SaveChanges();
	}

	public SipMethods[] FindAll()
	{
		using var ctx = _contextFactory.CreateContext();
		return ctx.Methods.Select(x => Enum.Parse<SipMethods>(x.Value)).ToArray();
	}
}
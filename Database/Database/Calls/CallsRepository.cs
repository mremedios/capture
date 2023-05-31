using System.Text.Json;
using System.Transactions;
using Database.Database.Calls.Models;
using Database.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Database.Database.Calls;

public class CallsRepository : ICallsRepository, IDisposable
{
    private readonly IContextFactory _contextFactory;
    private readonly IMemoryCache _cache;

    public CallsRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
        _cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
    }

    private CallsContext CreateContext()
    {
        return _contextFactory.CreateContext();
    }

    public async Task InsertRangeAsync(IList<Data> rawMessages)
    {
        using (var ctx = CreateContext())
        {
            var calls = rawMessages.Select(x => (x, GetCallCaching(ctx, x))).ToList();

            var t1 = PutMessages(ctx, calls);
            await PutHeaders(ctx, calls);
            await t1;

            ctx.SaveChanges();
        }
    }

    private Call? GetCallCaching(CallsContext ctx, Data data)
    {
        return _cache.GetOrCreate(data.CallId + data.Host,
            (r =>
            {
                r.Size = 1;
                r.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return InsertAndGetCall(ctx, data);
            })
        );
    }

    private Call InsertAndGetCall(CallsContext ctx, Data data)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, // PG may work 
            TransactionScopeAsyncFlowOption.Enabled);

        var callWithId = GetCall(ctx, data);

        if (callWithId != null)
        {
            return callWithId;
        }

        var call = new Call { Date = data.ReceivingTime, Host = data.Host.ToString(), CallId = data.CallId };
        ctx.Calls.Add(call);

        ctx.SaveChanges();
        transaction.Complete();

        return call;
    }


    private Call? GetCall(CallsContext ctx, Data data)
    {
        return ctx.Calls.FirstOrDefault(call => call.CallId == data.CallId &&
                                                call.Host == data.Host.ToString() &&
                                                call.Date > DateTime.Now.AddHours(-2)
        );
    }

    private async Task PutMessages(CallsContext ctx, IList<(Data, Call)> calls)
    {
        var messages = calls.Select(x =>
        {
            var (data, call) = x;
            return new Message
            {
                Text = data.SipMessage,
                Date = DateOnly.FromDateTime(call.Date),
                LocalCallId = call.LocalCallId,
                Details = JsonSerializer.Serialize(data.Details)
            };
        });

        await ctx.AddRangeAsync(messages);
    }

    private async Task PutHeaders(CallsContext ctx, IList<(Data, Call)> calls)
    {
        var headers = calls.SelectMany(x =>
            {
                var (data, call) = x;
                return data.Headers.Select(h =>
                    new CallHeader
                    {
                        Header = h.Key,
                        Value = h.Value,
                        LocalCallId = call.LocalCallId,
                        At = DateOnly.FromDateTime(call.Date)
                    });
            }
        ).Distinct().ToArray();

        await ctx.StoredProcedure($"insert_headers", headers);
    }

    public void Dispose()
    {
        _cache?.Dispose();
    }
}
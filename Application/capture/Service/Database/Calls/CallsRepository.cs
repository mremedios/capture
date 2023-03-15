using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Capture.Service.Database.Calls.Models;
using Capture.Service.Handler;

namespace Capture.Service.Database.Calls;

public class CallsRepository : IHeaderRepository
{
    private readonly IContextFactory _contextFactory;

    public CallsRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private CallsContext CreateContext()
    {
        return _contextFactory.CreateContext();
    }

    public async Task InsertRangeAsync(IList<Data> rawMessages)
    {
        using (var ctx1 = CreateContext())
        {
            var calls = rawMessages.Select(x => (x, InsertAndGetCall(ctx1, x).LocalCallId)).ToList();

            await PutMessages(ctx1, calls);
            await PutHeaders(ctx1, calls);

            await ctx1.SaveChangesAsync();
        }
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

        var call = new Call { Date = data.Time, Host = data.Host.ToString(), CallId = data.CallId };
        ctx.Calls.Add(call);

        ctx.SaveChanges();
        transaction.Complete();

        return call;
    }


    private Call GetCall(CallsContext ctx, Data data)
    {
        return ctx.Calls.FirstOrDefault(call => call.CallId == data.CallId &&
                                                call.Host == data.Host.ToString() &&
                                                call.Date > DateTime.Now.AddHours(-2)
        );
    }

    private async Task PutMessages(CallsContext ctx, IList<(Data, int)> calls)
    {
        var messages = calls.Select(x =>
        {
            var (data, localCallId) = x;
            return new Message
            {
                Headers = JsonSerializer.Serialize(data.Headers),
                Text = Encoding.Default.GetString(data.SipMessage),
                LocalCallId = localCallId
            };
        });

        await ctx.AddRangeAsync(messages);
    }

    private async Task PutHeaders(CallsContext ctx,  IList<(Data, int)> calls)
    {
        var headers = calls.SelectMany(x =>
            {
                var (data, localCallId) = x;
                return data.Headers.Select(h =>
                    new Header
                    {
                        header = h.Key,
                        Value = h.Value,
                        LocalCallId = localCallId
                    });
            }
        ).ToArray();

        await StoredProcedure.CallProcedure(ctx, "insert_headers", headers);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Capture.Service.Database.Calls.Models;
using Capture.Service.Models;

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
        using (var ctx = CreateContext())
        {
            var calls = rawMessages.Select(x => (x, InsertAndGetCall(ctx, x).LocalCallId)).ToList();

            await PutMessages(ctx, calls);
            await PutHeaders(ctx, calls);

            await ctx.SaveChangesAsync();
        }
    }

    public ShortData[] FindByHeader(string header)
    {
        using var ctx = CreateContext();
        var messages =
            ctx
                .Headers.Where(h => h.Value == header)
                .Join(
                    ctx.Messages,
                    h => h.LocalCallId,
                    m => m.LocalCallId,
                    (h, m) => m
                );

        var shortMsg = messages.ToList().Select(m =>
            new ShortData(
                m.Text,
                JsonSerializer.Deserialize<Details>(m.Details)
            )
        );

        return shortMsg.ToArray();
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
                Text = data.SipMessage,
                LocalCallId = localCallId,
                Details = JsonSerializer.Serialize(data.Details)
            };
        });

        await ctx.AddRangeAsync(messages);
    }

    private async Task PutHeaders(CallsContext ctx, IList<(Data, int)> calls)
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
        ).Distinct().ToArray();

        await ctx.StoredProcedure("insert_headers", headers);
    }
}
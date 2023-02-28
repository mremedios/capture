using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Capture.Service.Database.Calls.Entities;
using Capture.Service.NameLater;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Capture.Service.Database.Calls;

public class CallsRepository : IHeaderRepository
{
    public async Task InsertRangeAsync(IList<NameIt> nameIts)
    {
        var calls = nameIts.Select(x => InsertAndGetCall(x).LocalCallId).ToList();

        var messages = nameIts.Zip(calls).Select(x =>
        {
            var (nameIt, localCallId) = x;
            return new Message
            {
                Headers = JsonConvert.SerializeObject(nameIt.Headers),
                message = System.Text.Encoding.Default.GetString(nameIt.SipMessage),
                LocalCallId = localCallId
            };
        });

        var headers = nameIts.Zip(calls).SelectMany(x =>
            {
                var (nameIt, localCallId) = x;
                return nameIt.Headers.Select(h =>
                    new Header
                    {
                        header = h.Key,
                        Value = h.Value,
                        LocalCallId = localCallId
                    });
            }
        ).ToList();

        await using var ctx = new CallsContext();
        ctx.Messages.AddRange(messages);
        await ctx.SaveChangesAsync();

        CallProcedure("insert_data", headers);
    }

    public string[] FindByHeader(string key, string value)
    {
        throw new System.NotImplementedException();
    }

    public string[] AvailableHeaders()
    {
        using var ctx = new CallsContext();
        return ctx.AvailableHeaders.Select(x => x.Header).ToArray();
    }

    private Call InsertAndGetCall(NameIt nameIt)
    {
        using var ctx = new CallsContext();

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, // PG may work 
            TransactionScopeAsyncFlowOption.Enabled);

        var callWithId = GetCall(ctx, nameIt);

        if (callWithId != null)
        {
            return callWithId;
        }

        var call = new Call { Date = DateTime.Now, Host = nameIt.Host.ToString(), CallId = nameIt.CallId };
        ctx.Calls.Add(call);

        ctx.SaveChanges();
        transaction.Complete();

        return call;
    }


    private Call GetCall(CallsContext ctx, NameIt nameIt)
    {
        return ctx.Calls.FirstOrDefault(call => call.CallId == nameIt.CallId &&
                                                call.Host == nameIt.Host.ToString() &&
                                                call.Date > DateTime.Now.AddHours(-2)
        );
    }

    private void CallProcedure<T>(string procedureName, T data)
    {
        using var ctx = new CallsContext();
        using var connection = (NpgsqlConnection)ctx.Database.GetDbConnection();

        connection.Open();
        using var command = new NpgsqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            Parameters =
            {
                new() { Value = data }
            }
        };  

        command.ExecuteNonQuery();
    }
}
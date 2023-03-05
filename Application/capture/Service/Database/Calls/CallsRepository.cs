using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Capture.Service.Database.Calls.Entities;
using Capture.Service.Handler;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Capture.Service.Database.Calls;

public class CallsRepository : IHeaderRepository
{
    public async Task InsertRangeAsync(IList<Data> rawMessages)
    {

        AvailableHeaders();
        // var calls = rawMessages.Select(x => InsertAndGetCall(x).LocalCallId).ToList();

        // var messages = rawMessages.Zip(calls).Select(x =>
        // {
        //     var (data, localCallId) = x;
        //     return new Message
        //     {
        //         Headers = JsonConvert.SerializeObject(data.Headers),
        //         message = System.Text.Encoding.Default.GetString(data.SipMessage),
        //         LocalCallId = localCallId
        //     };
        // });
        //
        // var headers = rawMessages.Zip(calls).SelectMany(x =>
        //     {
        //         var (data, localCallId) = x;
        //         return data.Headers.Select(h =>
        //             new Header
        //             {
        //                 header = h.Key,
        //                 Value = h.Value,
        //                 LocalCallId = localCallId
        //             });
        //     }
        // ).ToList();

        // using (var ctx = new CallsContext())
        // {
        //     ctx.AddRange(messages);
        //     await ctx.SaveChangesAsync();
        // }
        //
        // CallProcedure("insert_data", headers);
    }

    public string[] FindByHeader(string key, string value)
    {
        throw new System.NotImplementedException();
    }

    public string[] AvailableHeaders()
    {
        using var ctx = new CallsContext();
        return ctx.AvailableHeaders.Select(x => x.Header).ToArray();
        // return new string[] { };
    }

    private Call InsertAndGetCall(Data data)
    {
        using (var transaction = new TransactionScope(
                   TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, // PG may work 
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            using var ctx = new CallsContext();
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
    }


    private Call GetCall(CallsContext ctx, Data data)
    {
        return ctx.Calls.FirstOrDefault(call => call.CallId == data.CallId &&
                                                call.Host == data.Host.ToString() &&
                                                call.Date > DateTime.Now.AddHours(-2)
        );
    }

    private void CallProcedure<T>(string procedureName, T data)
    {
        using var ctx = new CallsContext();
        using (var connection = (NpgsqlConnection)ctx.Database.GetDbConnection())
        {
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
            connection.Close();
        }
    }
}
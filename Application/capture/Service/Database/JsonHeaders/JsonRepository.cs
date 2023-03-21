using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Service.Database.JsonHeaders.Entities;
using Capture.Service.Handler;
using Capture.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database.JsonHeaders;

public class JsonRepository : IHeaderRepository
{
    public async Task InsertRangeAsync(IList<Data> rawMessages)
    {
        using (var ctx = new JsonContext())
        {
            try
            {
                await ctx.AddRangeAsync(rawMessages.Select(GetHeader));
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(":/ " + e.Message);
            }
        }
    }
    

    public ShortData[] FindByHeader(string value)
    {
        throw new NotImplementedException();
        // using (var ctx = new JsonContext())
        // {
        //     var search = $"\"callid\": \"{value}\"";
        //     var res = ctx.Headers
        //         .Where(h => EF.Functions.JsonContains(h.protocol_header, search))
        //         .ToList();
        //     return res.Select(h => h.raw).ToArray();
        // }
    }

    private static Header GetHeader(Data data)
    {
        return new Header
        {
            created_date = data.ReceivingTime,
            call_id = data.CallId,
            endpoint = data.Host.ToString(),
            raw = data.SipMessage,
            protocol_header = data.Headers
        };
    }
}
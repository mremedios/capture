using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Service.Database.JsonHeaders.Entities;
using Capture.Service.Handler;
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

    public string[] FindByHeader(string key, string value)
    {
        using (var ctx = new JsonContext())
        {
            var search = $"\"{key}\": \"{value}\"";
            var res = ctx.Headers
                .Where(h => EF.Functions.JsonContains(h.protocol_header, search))
                .ToList();
            return res.Select(h => h.raw).ToArray();
        }
    }

    public string[] AvailableHeaders()
    {
        using (var ctx = new JsonContext())
        {
            return ctx.AvailableHeaders.Select(x => x.Header).ToArray();
        }
    }
    
    private static Header GetHeader(Data data)
    {
        return new Header
        {
            created_date = data.Time,
            call_id = data.CallId,
            endpoint = data.Host.ToString(),
            raw = Encoding.Default.GetString(data.SipMessage),
            protocol_header = data.Headers
        };
    }
    
}
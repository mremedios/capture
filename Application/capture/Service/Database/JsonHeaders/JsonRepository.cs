using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capture.Service.NameLater;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database.JsonHeaders;

public class JsonRepository : IHeaderRepository
{
    public async Task InsertRangeAsync(IList<NameIt> nameIt)
    {
        using (var ctx = new JsonContext())
        {
            try
            {
                await ctx.AddRangeAsync(nameIt.Select(GetHeader));
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
    
    private static Header GetHeader(NameIt nameIt)
    {
        return new Header
        {
            created_date = nameIt.Time,
            call_id = nameIt.CallId,
            endpoint = nameIt.Host.ToString(),
            raw = System.Text.Encoding.Default.GetString(nameIt.SipMessage),
            protocol_header = nameIt.Headers
        };
    }
    
}
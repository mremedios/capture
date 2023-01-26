using System;
using System.Linq;
using System.Threading.Tasks;
using Capture.Service.NameLater;
using Microsoft.EntityFrameworkCore;

namespace Capture.Service.Database.JsonHeaders;

public class JsonRepository : IHeaderRepository
{
    public async Task Insert(NameIt nameIt)
    {
        using (var ctx = new JsonContext())
        {
            try
            {
                await ctx.AddAsync(GetHeader(nameIt));
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(":/ " + e.Message);
            }
        }
    }

    public string[] Select()
    {
        using (var ctx = new JsonContext())
        {
            var search = "[{\"tcommuniactionid\": \"b5d3756cbd3b43929a9422796d1db48a\"}]";
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

            var n = ctx.AvailableHeaders.FirstOrDefault();
            var a = ctx.AvailableHeaders.Where(x => true).ToArray();
            var b = a.Select(x => x.Header);
            return b.ToArray();
        }
    }
    
    private static Header GetHeader(NameIt nameIt)
    {
        return new Header
        {
            created_date = nameIt.Time,
            call_id = nameIt.CallId,
            endpoint = nameIt.ServerIp.ToString(),
            raw = System.Text.Encoding.Default.GetString(nameIt.SipMessage),
            protocol_header = nameIt.Headers
        };
    }
    
}
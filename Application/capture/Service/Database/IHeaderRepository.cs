using System.Collections.Generic;
using System.Threading.Tasks;
using Capture.Service.Database.Calls.Models;
using Capture.Service.Handler;
using Capture.Service.Models;

namespace Capture.Service.Database;

public interface IHeaderRepository
{
    public Task InsertRangeAsync(IList<Data> rawMessages);

    public ShortData[] FindByHeader(string header);
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Capture.Service.Handler;

namespace Capture.Service.Database;

public interface IHeaderRepository
{
    public Task InsertRangeAsync(IList<Data> rawMessages);

    public string[] FindAvailableHeaders();

}
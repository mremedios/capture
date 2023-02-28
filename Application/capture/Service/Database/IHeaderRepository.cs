using System.Collections.Generic;
using System.Threading.Tasks;
using Capture.Service.NameLater;

namespace Capture.Service.Database;

public interface IHeaderRepository
{
    public Task InsertRangeAsync(IList<NameIt> nameIt);

    public string[] FindByHeader(string key, string value);

    public string[] AvailableHeaders();
}
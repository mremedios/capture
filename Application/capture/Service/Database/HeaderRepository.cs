using System.Threading.Tasks;
using Capture.Service.NameLater;

namespace Capture.Service.Database;

public interface IHeaderRepository
{
    public Task Insert(NameIt nameIt);

    public string[] Select();

    public string[] AvailableHeaders();
}
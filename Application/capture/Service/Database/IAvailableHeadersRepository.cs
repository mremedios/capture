using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Database;

// называется как-то упорото
public interface IAvailableHeadersRepository
{
    public ISet<string> GetAvailableHeaders();

    public Task StartAsync(CancellationToken ct);
    public Task StopAsync(CancellationToken ct);
}
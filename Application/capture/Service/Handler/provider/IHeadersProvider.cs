using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Handler.provider;

public interface IHeadersProvider
{
    public ISet<string> GetAvailableHeaders();

    public Task StartAsync(CancellationToken ct);
    
    public Task StopAsync(CancellationToken ct);
}
using Capture.Service.Listener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Database;
using Capture.Service.Handler;

namespace Capture.Service;

public class HostedService : IHostedService
{
    private readonly ICapture[] _captures;
    private readonly IAvailableHeadersRepository _rep;
    private CancellationTokenSource _cts;

    public HostedService(IEnumerable<ICapture> captures, IAvailableHeadersRepository rep)
    {
        _captures = captures.ToArray();
        _rep = rep;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var startTasks = _captures
            .Select(x => x.StartAsync(_cts.Token))
            .Append(_rep.StartAsync(_cts.Token))
            .ToArray();

        return startTasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(startTasks);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _rep.StopAsync(cancellationToken);
        return Task.CompletedTask;
    }
}
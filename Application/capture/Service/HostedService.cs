using Capture.Service.Listener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Handler;

namespace Capture.Service;

public class HostedService : IHostedService
{
    private readonly ICapture[] _captures;
    private CancellationTokenSource _cts;
    private IHandler _handler;

    public HostedService(IConfiguration config, ILogger<HostedService> logger, IEnumerable<ICapture> captures,
        IHandler handler)
    {
        _captures = captures.ToArray();
        _handler = handler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var startTasks = _captures.Select(x => x.StartAsync(_cts.Token))
            .ToArray();

        return startTasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(startTasks);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
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
using Capture.Service.Handler.provider;

namespace Capture.Service;

public class HostedService : IHostedService
{
    private readonly ICapture[] _captures;
    private readonly IHeadersProvider _provider;
    private CancellationTokenSource _cts;

    public HostedService(IEnumerable<ICapture> captures, IHeadersProvider provider)
    {
        _captures = captures.ToArray();
        _provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var startTasks = _captures
            .Select(x => x.StartAsync(_cts.Token))
            .Append(_provider.StartAsync(_cts.Token))
            .ToArray();

        return startTasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(startTasks);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _provider.StopAsync(cancellationToken);
        return Task.CompletedTask;
    }
}
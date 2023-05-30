using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Handler;
using Capture.Service.Handler.provider;
using Microsoft.Extensions.Hosting;

namespace Capture.Service;

public class HostedService : IHostedService
{
    private readonly IEnumerable<ICapture> _captures;
    private readonly IOptionsProvider _provider;
    private CancellationTokenSource _cts;
    private IHandler _handler;
    
    public HostedService(IEnumerable<ICapture> captures, IOptionsProvider provider, IHandler handler)
    {
        _captures = captures;
        _provider = provider;
        _handler = handler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var startTasks = _captures
            .Select(x => x.StartAsync(_cts.Token))
            .Append(_provider.StartAsync(_cts.Token))
            .Concat(_handler.StartAsync(_cts.Token))
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
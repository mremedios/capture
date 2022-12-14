using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Capture.Service.Listener;

namespace Capture.Service;

public class HostedService: IHostedService
{
    private readonly ICapture _capture;
    private CancellationTokenSource _cts;
    
    public HostedService(IConfiguration config, ILogger<UdpCapture> logger, ICapture capture)
    {
        _capture = capture;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return _capture.StartAsync(_cts.Token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
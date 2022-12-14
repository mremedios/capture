using Capture.Service.Listener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service;

public class HostedService : IHostedService
{
	private readonly ICapture[] _captures;
	private CancellationTokenSource _cts;

	public HostedService(IConfiguration config, ILogger<UdpCapture> logger, IEnumerable<ICapture> captures)
	{
		_captures = captures.ToArray();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		var startTasks = _captures.Select(x => x.StartAsync(_cts.Token));

		//Напомни, скину ссылку на безопасный WhenAll

		return Task.WhenAll(startTasks);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_cts.Cancel();
		return Task.CompletedTask;
	}
}
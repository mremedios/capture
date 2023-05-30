using Capture.Service.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Listener
{
	public class UdpCapture : ICapture
	{
		private readonly CaptureConfiguration _config;
		private readonly ILogger<UdpCapture> _logger;
		private CancellationTokenSource _cts;
		private IHandler _handler;

		private UdpClient _listener;
		private UdpClient _homerClient;
		private Task _task;

		private int _count = 0;

		public UdpCapture(IConfiguration config, ILogger<UdpCapture> logger, IHandler handler)
		{
			_logger = logger;
			_config = config.GetSection("Capture").Get<CaptureConfiguration>();
			_handler = handler;
		}

		public void Dispose()
		{
			_listener.Dispose();
			_homerClient?.Dispose();
			_cts?.Dispose();
		}

		public Task StartAsync(CancellationToken ct)
		{
			_cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

			var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort);

			_listener = new UdpClient(listenEndpoint);

			_logger.LogInformation("Start udp listener");
			_task = CaptureAsync();

			return Task.CompletedTask;
		}

		private async Task Reсeive()
		{
			var x = await _listener.ReceiveAsync(_cts.Token);
			_handler.HandleMessage(x);
		}

		private async Task CaptureAsync()
		{
			var counter = 1;
			try
			{
				while (!_cts.Token.IsCancellationRequested)
				{
					await Reсeive();
					counter += 1;
					if (counter % 100 == 0) Console.WriteLine(counter);
				}
			}
			catch (Exception e)
			{
				_logger.LogWarning(e.Message);
			}
		}
	}
}
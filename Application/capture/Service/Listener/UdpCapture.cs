using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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

		private UdpClient _listener;
		private UdpClient _homerClient;
		private Task _task;

		public UdpCapture(IConfiguration config, ILogger<UdpCapture> logger)
		{
			_logger = logger;
			_config = config.GetSection("Capture").Get<CaptureConfiguration>();
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

		private async Task CaptureAsync()
		{
			var fileName = $"HEP_sample_{DateTime.Now:yyyyMMdd_hhmmss}.bin";
			var fileOutputStream = File.OpenWrite(fileName);

			try
			{
				while (!_cts.Token.IsCancellationRequested)
				{
					var x = await _listener.ReceiveAsync(_cts.Token);
					_logger.LogInformation("Handle message from {}", x.RemoteEndPoint);

					await fileOutputStream.WriteAsync(x.Buffer, _cts.Token);
				}
			}
			catch (Exception e)
			{
				_logger.LogWarning(e.Message);
			}
			finally
			{
				await fileOutputStream.DisposeAsync();
			}
		}
	}
}
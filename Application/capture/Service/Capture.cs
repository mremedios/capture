using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Capture.Service
{
	public class Capture : ICapture
	{
		private readonly CaptureConfiguration _config;
		private readonly ILogger<Capture> _logger;
		private CancellationTokenSource _cts;

		private TcpListener _tcpListener;
		private TcpClient _homerClient;
		private Task _task;
		
		public Capture(IConfiguration config, ILogger<Capture> logger)
		{
			_logger = logger;
			_config = config.GetSection("Capture").Get<CaptureConfiguration>(); 
		}
		
		public void Dispose()
		{
			_homerClient?.Close();
			_homerClient?.Dispose();
			_cts?.Dispose();
		}

		public async Task StartAsync(CancellationToken ct)
		{
			_logger.LogInformation("Started");
			_cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

			var homerEndpoint = new IPEndPoint(IPAddress.Parse(_config.HomerAddress), _config.HomerPort);
			var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.Port);
			
			_tcpListener = new TcpListener(listenEndpoint);
			_homerClient = new TcpClient();
			
			await _homerClient.ConnectAsync(homerEndpoint, ct);
			_tcpListener.Start();
			
			_task = CaptureAsync();
		}
		private async Task CaptureAsync()
		{
			var fileName = $"HEP_sample_{DateTime.Now:yyyyMMdd_hhmmss}.bin";
			var ct = _cts.Token;
			
			var fileOutput = File.OpenWrite(fileName);
			var client = await _tcpListener.AcceptTcpClientAsync(ct);
			var inputStream = client.GetStream();
			var outputStream = _homerClient.GetStream();

			try
			{
				var read = 1;
				while (!ct.IsCancellationRequested && read > 0)
				{
					using (var buffer = MemoryPool<byte>.Shared.Rent(1024))
					{
						_logger.LogInformation("Handle");
						read = await inputStream.ReadAsync(buffer.Memory, ct);
						var firstTasks = fileOutput.WriteAsync(buffer.Memory.Slice(0, read), ct);
						await outputStream.WriteAsync(buffer.Memory.Slice(0, read), ct);
						await firstTasks;
					}
				}

				fileOutput.Close();
				client.Close();
				inputStream.Close();
				outputStream.Close();
			}
			finally
			{
				await fileOutput.DisposeAsync();
				client.Dispose();
				await inputStream.DisposeAsync();
				await outputStream.DisposeAsync();
			}
		}

	}
}
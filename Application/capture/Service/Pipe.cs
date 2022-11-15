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
			_logger.LogInformation(_config.ToString());
			_cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

			var homerEndpoint = new IPEndPoint(IPAddress.Parse(_config.HomerAddress), _config.HomerPort);
			var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.Port);
			
			_tcpListener = new TcpListener(listenEndpoint);
			_homerClient = new TcpClient();
			
			await _homerClient.ConnectAsync(homerEndpoint);
			_tcpListener.Start();
			
			_task = CaptureAsync();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		private async Task CaptureAsync()
		{
			var fileName = $"HEP_sample_{DateTime.Now:yyyyMMdd_hhmmss}.bin";
			var ct = _cts.Token;
			var fileOutput = File.OpenWrite(fileName);
			var client = await _tcpListener.AcceptTcpClientAsync();
			var inputStream = client.GetStream();
			var outputStream = _homerClient.GetStream();

			try
			{
				ValueTask firstTasks;
				while (!ct.IsCancellationRequested)
				{
					using (var buffer = MemoryPool<byte>.Shared.Rent(1024))
					{
						_logger.LogInformation("Handle");
						var readed = await inputStream.ReadAsync(buffer.Memory, ct);
						firstTasks = fileOutput.WriteAsync(buffer.Memory.Slice(0, readed), ct);
						await outputStream.WriteAsync(buffer.Memory.Slice(0, readed), ct);
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
				fileOutput.Dispose();
				client.Dispose();
				inputStream.Dispose();
				outputStream.Dispose();
			}
		}

	}
}
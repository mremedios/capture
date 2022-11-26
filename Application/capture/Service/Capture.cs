using System;
using System.Buffers;
using System.IO;
using System.Linq;
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
            _logger.LogInformation("T3");
            _tcpListener.Stop();
            _homerClient?.Dispose();
            _cts?.Dispose();
        }

        public async Task StartAsync(CancellationToken ct)
        {
            _logger.LogInformation("Started");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var homerEndpoint = new IPEndPoint(IPAddress.Parse(_config.HomerAddress), _config.HomerPort);
            var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort);

            _tcpListener = new TcpListener(listenEndpoint);
            _homerClient = new TcpClient();

            await _homerClient.ConnectAsync(homerEndpoint, ct);
            _tcpListener.Start();

            _task = CaptureAsync();
        }

        private async Task CaptureAsync()
        {
            var fileName = $"HEP_sample_{DateTime.Now:yyyyMMdd_hhmmss}.bin";
            var fileOutputStream = File.OpenWrite(fileName);
            var outputStream = _homerClient.GetStream();
            
            Stream[] output = { fileOutputStream, outputStream };
            
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await AcceptClient(output);
                }
            }
            finally
            {
                await fileOutputStream.DisposeAsync();
                await outputStream.DisposeAsync();
            }
        }

        private async Task AcceptClient(Stream[] output)
        {
            _logger.LogInformation("Waiting for a connection");
            var client = await _tcpListener.AcceptTcpClientAsync(_cts.Token);
            var inputStream = client.GetStream();

            try
            {
                using (var buffer = MemoryPool<byte>.Shared.Rent(1024))
                {
                    int read;
                    while ((read = await inputStream.ReadAsync(buffer.Memory, _cts.Token)) > 0)
                    {
                        _logger.LogInformation("Handle");
                        var tasks = output.Select(st =>
                            st.WriteAsync(buffer.Memory.Slice(0, read), _cts.Token).AsTask()
                        );
                        await Task.WhenAll(tasks);
                    }
                }
            }
            finally
            {
                Array.ForEach(output, st => st.Flush());
                await inputStream.DisposeAsync();
                client.Dispose();
            }
        }
    }
}
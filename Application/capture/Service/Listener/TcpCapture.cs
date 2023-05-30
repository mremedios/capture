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

namespace Capture.Service.Listener
{
    public class TcpCapture : ICapture
    {
        private readonly CaptureConfiguration _config;
        private readonly ILogger<TcpCapture> _logger;
        private CancellationTokenSource _cts;

        private TcpListener _tcpListener;
        private TcpClient _homerClient;
        private Task _task;

        public TcpCapture(IConfiguration config, ILogger<TcpCapture> logger)
        {
            _logger = logger;
            _config = config.GetSection("Capture").Get<CaptureConfiguration>();
        }

        public void Dispose()
        {
            _tcpListener.Stop();
            _homerClient?.Dispose();
            _cts?.Dispose();
        }

        public async Task StartAsync(CancellationToken ct)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var homerEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort); // TODO remove or fix
            var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort);

            _tcpListener = new TcpListener(listenEndpoint);
            _homerClient = new TcpClient();

            await _homerClient.ConnectAsync(homerEndpoint, ct);
            _tcpListener.Start();

            _logger.LogInformation("Start tcp listener");
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
                    var client = await _tcpListener.AcceptTcpClientAsync(_cts.Token);
                    AcceptClient(client, output);
                }
            }
            finally
            {
                await fileOutputStream.DisposeAsync();
                await outputStream.DisposeAsync();
            }
        }

        private async Task AcceptClient(TcpClient client, Stream[] output)
        {
            _logger.LogInformation("Accept from {}", client.Client.AddressFamily);
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
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
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
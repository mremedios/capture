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

        private UdpClient _listener;
        private UdpClient _homerClient;
        private Task _task;

        public Capture(IConfiguration config, ILogger<Capture> logger)
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

        public async Task StartAsync(CancellationToken ct)
        {
            _logger.LogInformation("Started");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort);

            _listener = new UdpClient(listenEndpoint);

            _homerClient = new UdpClient();

            _task = CaptureAsync();
        }

        private async Task CaptureAsync()
        {
            var fileName = $"HEP_sample_{DateTime.Now:yyyyMMdd_hhmmss}.bin";
            var fileOutputStream = File.OpenWrite(fileName);

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await AcceptClient(_listener, fileOutputStream, _homerClient);
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

        private async Task AcceptClient(UdpClient client, Stream output, UdpClient homer)
        {
            var homerEndpoint = new IPEndPoint(IPAddress.Parse(_config.HomerAddress), _config.HomerPort);
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting");
                    var x = await client.ReceiveAsync(_cts.Token);
                    _logger.LogInformation("Handle {}" ,x.RemoteEndPoint.ToString());

                    await output.WriteAsync(x.Buffer, _cts.Token);
                    await homer.SendAsync(x.Buffer, homerEndpoint, _cts.Token);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
            }
            finally
            {
                output.Flush();
            }
        }
    }
}
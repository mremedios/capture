﻿using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.NameLater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        public async Task StartAsync(CancellationToken ct)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var listenEndpoint = new IPEndPoint(IPAddress.Any, _config.ListenPort);

            _listener = new UdpClient(listenEndpoint);

            _logger.LogInformation("Start udp listener");
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
                    var x = await _listener.ReceiveAsync(_cts.Token);
                    _logger.LogInformation("Handle message from {}" ,x.RemoteEndPoint);
                    
                    _handler.HandleMessage(x.Buffer);
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
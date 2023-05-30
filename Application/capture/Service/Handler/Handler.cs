using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Capture.Service.Handler.provider;
using Capture.Service.Listener;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Database.Database;
using Database.Models;
using Microsoft.Extensions.Logging;

namespace Capture.Service.Handler;

public class Handler : IHandler, IDisposable
{
    private readonly ILogger<Handler> _logger;
    private readonly ICallsRepository _repository;
    private readonly BufferedTaskQueue<Data> _dbQueue;
    private readonly IOptionsProvider _provider;

    private const int BufferSize = 1000;

    private readonly Channel<ReceivedData> _channel = Channel.CreateUnbounded<ReceivedData>(
        new UnboundedChannelOptions
        {
            SingleWriter = true,
            SingleReader = false,
        });

    private ChannelWriter<ReceivedData> writer;
    private IEnumerable<ChannelReader<ReceivedData>> _readers;
    private CancellationTokenSource _cts;

    public Handler(ILogger<Handler> logger,
        ICallsRepository repository,
        IOptionsProvider provider)
    {
        _logger = logger;
        _repository = repository;
        _dbQueue = new BufferedTaskQueue<Data>(Save, bufferSize: BufferSize, exceptionHandler: SavingErrorHandler);
        _provider = provider;
        writer = _channel;
        _readers = new[] { _channel.Reader, _channel.Reader };
    }


    public IEnumerable<Task> StartAsync(CancellationToken ct)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _logger.LogInformation("Start consumers");
        var tasks = _readers.Select(ConsumeAsync);
        return tasks;
    }
    
    private async Task ConsumeAsync(ChannelReader<ReceivedData> reader, int i)
    {
        try
        {
            while (await reader.WaitToReadAsync(_cts.Token))
            {
                while (reader.TryRead(out ReceivedData data))
                {
                    Parse(data);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation("Consumer error: {0}", e);
        }
    }

    private void Parse(ReceivedData data)
    {
        try
        {
            var message = ParserHePv3.ParseMessage(data.Msg);
            var sipMethod = message.Sip.CSeqMethod.ToModel();
            if (!_provider.GetExcludedMethods().Contains(sipMethod))
            {
                var d = GetData(message, data.EndPoint, data.Time);
                _dbQueue.EnqueueTask(d);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning("Error parsing message: {0}", e.Message);
        }
    }

    private async Task ParseAndSave(IList<ReceivedData> data)
    {
        var toSave = data.Select(data =>
            {
                var message = ParserHePv3.ParseMessage(data.Msg);
                return (data, message);
            }).Where(dm =>
                !_provider.GetExcludedMethods().Contains(dm.message.Sip.CSeqMethod.ToModel()))
            .Select(dm => GetData(dm.message, dm.data.EndPoint, dm.data.Time));

        await _repository.InsertRangeAsync(toSave.ToList());
    }


    private void SavingErrorHandler(Exception e, IList<Data> y)
    {
        _logger.LogWarning("Error saving message: {0}", e.Message);
    }

    private async Task Save(IList<Data> data)
    {
        await _repository.InsertRangeAsync(data);
    }

    public void HandleMessage(ReceivedData r)
    {
        var res = writer.TryWrite(r);
        if (res == false)
        {
            _logger.LogCritical("Package loss");
        }
    }

    private Data GetData(Message msg, IPEndPoint endPoint, DateTime time)
    {
        Dictionary<string, string> headers = new();

        foreach (var h in msg.Sip.UnknownHeaders)
        {
            var (key, value) = ParseUnknownHeader(h);
            if (_provider.GetAvailableHeaders().Contains(key))
            {
                headers[key] = value;
            }
        }

        var details = new Details(
            msg.Hep.sourceIPAddress,
            msg.Hep.destinationIPAddress,
            msg.Hep.timeSeconds,
            msg.Hep.timeUseconds);

        return new Data(
            headers,
            Encoding.Default.GetString(msg.Payload),
            msg.Sip.CallId,
            endPoint,
            time,
            details);
    }

    private static (string, string) ParseUnknownHeader(string str)
    {
        var x = str.Split(':');
        return (x[0].Trim(), x[1].Trim());
    }

    public void Dispose()
    {
        _dbQueue?.Dispose();
    }
}
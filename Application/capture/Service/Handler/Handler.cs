using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Database;
using Capture.Service.Handler.provider;
using Capture.Service.Listener;
using Capture.Service.Models;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Microsoft.Extensions.Logging;

namespace Capture.Service.Handler;

public class Handler : IHandler, IDisposable
{
    private readonly ILogger<Handler> _logger;
    private readonly ICallsRepository _repository;
    private readonly TaskQueue<ReceivedData> _parseQueue;
    private readonly BufferedTaskQueue<Data> _dbQueue;
    private readonly IOptionsProvider _provider;
    private readonly Timer _timer;

    public Handler(ILogger<Handler> logger, ICallsRepository repository, IOptionsProvider provider)
    {
        _logger = logger;
        _repository = repository;
        _parseQueue = new TaskQueue<ReceivedData>(Parse, ParsingErrorHandler);
        _dbQueue = new BufferedTaskQueue<Data>(Save, bufferSize: 1000, exceptionHandler: SavingErrorHandler);
        _provider = provider;
        _timer = new Timer((e) => { _logger.LogDebug("Parser queue size {0}", _parseQueue.TaskCount); }, null,
            TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private void Parse(ReceivedData data)
    {
        var message = ParserHePv3.ParseMessage(data.Msg);
        if (!_provider.GetExcludedMethods().Contains(message.Sip.CSeqMethod.ToString()))
        {
            var d = GetData(message, data.EndPoint, data.Time);
            _dbQueue.EnqueueTask(d);
        }
    }

    private void ParsingErrorHandler(Exception e, ReceivedData y)
    {
        _logger.LogWarning("Error parsing message: {0}", e.Message);
    }

    private void SavingErrorHandler(Exception e, IList<Data> y)
    {
        _logger.LogWarning("Error saving message: {0}", e.Message);
    }

    private async Task Save(IList<Data> data)
    {
        await _repository.InsertRangeAsync(data);
    }

    public void HandleMessage(ReceivedData data)
    {
        _parseQueue.EnqueueTask(data);
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
        _timer.Change(Timeout.Infinite, 0);
        _timer.Dispose();
        _parseQueue?.Dispose();
        _dbQueue?.Dispose();
    }
}
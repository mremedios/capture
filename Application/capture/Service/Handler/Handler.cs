using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Capture.Service.Database;
using Capture.Service.Handler.provider;
using Capture.Service.Listener;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Microsoft.Extensions.Logging;

namespace Capture.Service.Handler;

public class Handler : IHandler
{
    private readonly ILogger<Handler> _logger;
    private readonly IHeaderRepository _repository;
    private readonly TaskQueue<ReceivedData> _parseQueue;
    private readonly BufferedTaskQueue<Data> _dbQueue;
    private readonly IHeadersProvider _provider;

    public Handler(ILogger<Handler> logger, IHeaderRepository repository, IHeadersProvider provider)
    {
        _logger = logger;
        _repository = repository;
        _parseQueue = new TaskQueue<ReceivedData>(Parse, ParsingErrorHandler);
        _dbQueue = new BufferedTaskQueue<Data>(Save, bufferSize: 1000, exceptionHandler: SavingErrorHandler);
        _provider = provider;
    }

    private void Parse(ReceivedData data)
    {
        var message = ParserHePv3.ParseMessage(data.Msg);
        var d = GetData(message, data.EndPoint, data.Time);
        _dbQueue.EnqueueTask(d);
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

    private Data GetData(Message msg, IPEndPoint endPoint, DateTime Time)
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

        return new Data(
            headers,
            msg.Payload,
            msg.Sip.CallId,
            endPoint,
            Time);
    }

    private static (string, string) ParseUnknownHeader(string str)
    {
        var x = str.Split(':');
        x[0] = x[0]
            .Replace("I-", "")
            .Replace("X-", "")
            .Replace("T-", "");
        return (x[0].ToLower().Trim(), x[1].Trim());
    }
}
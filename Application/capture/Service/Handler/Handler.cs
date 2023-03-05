using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Database;
using Capture.Service.Listener;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Microsoft.Extensions.Logging;
using SIPSorcery.SIP;

namespace Capture.Service.Handler;

public class Handler : IHandler
{
    private readonly ILogger<Handler> _logger;
    private readonly IHeaderRepository _repository;
    private readonly TaskQueue<ReceivedData> _parseQueue;
    private readonly BufferedTaskQueue<Data> _dbQueue;
    private ISet<string> _availableHeaders;

    public Handler(ILogger<Handler> logger, IHeaderRepository repository)
    {
        _logger = logger;
        _repository = repository;
        _parseQueue = new TaskQueue<ReceivedData>(Parse, ErrorHandler);
        _dbQueue = new BufferedTaskQueue<Data>(Save, bufferSize: 10);
        // UpdateHeaders();
    }

    public async Task UpdateHeaders()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        while (await timer.WaitForNextTickAsync())
        {
            _availableHeaders = new HashSet<string>(_repository.AvailableHeaders());
        }
    }

    private void Parse(ReceivedData data)
    {
        var message = ParserHePv3.ParseMessage(data.Msg);
        var nameIt = GetNameIt(message, data.EndPoint, data.Time);
        _dbQueue.EnqueueTask(nameIt);
    }

    private void ErrorHandler(Exception e, ReceivedData y)
    {
        //  Object reference not set to an instance of an object
        _logger.LogWarning("Smth went wrong {0}", e.Message);
    }

    private async Task Save(IList<Data> data)
    {
        await _repository.InsertRangeAsync(data);
    }

    public void HandleMessage(ReceivedData data)
    {
        _parseQueue.EnqueueTask(data);
    }

    private Data GetNameIt(Message msg, IPEndPoint endPoint, DateTime Time)
    {
        Dictionary<string, string> headers = new();
        foreach (var h in msg.Sip.UnknownHeaders)
        {
            var (key, value) = ParseUnknownHeader(h);
            if (_availableHeaders.Contains(key))
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

    private (string, string) ParseUnknownHeader(string str)
    {
        var x = str.Split(':');
        x[0] = x[0].Replace("I-", "").Replace("X-", "");
        return (x[0].ToLower().Trim(), x[1].Trim());
    }
}
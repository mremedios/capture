using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Capture.Service.Database;
using Capture.Service.Listener;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Microsoft.Extensions.Logging;

namespace Capture.Service.NameLater;

public class Handler : IHandler
{
    private readonly ILogger<Handler> _logger;
    private readonly IHeaderRepository _repository;
    private readonly TaskQueue<ReceivedData> _parseQueue; 
    private readonly BufferedTaskQueue<NameIt> _dbQueue; 

    public Handler(ILogger<Handler> logger, IHeaderRepository repository)
    {
        _logger = logger;
        _repository = repository;
        _parseQueue = new TaskQueue<ReceivedData>(Parse, ErrorHandler);
        _dbQueue = new BufferedTaskQueue<NameIt>(Save);
    }
    private void Parse(ReceivedData data)
    {
        var message = ParserHePv3.ParseMessage(data.Msg);
        var nameIt = GetNameIt(message, data.EndPoint, data.Time);
        _dbQueue.EnqueueTask(nameIt);
    }
    private void ErrorHandler(Exception e, ReceivedData y)
    {
        _logger.LogWarning("Smth went wrong {}", e.Message);
    }

    private async Task Save(IList<NameIt> nameIt)
    {
       // await _repository.Insert(nameIt); // тут должны быть батчи
       await Task.CompletedTask;
    }
    
    public void HandleMessage(ReceivedData data) // todo void?
    {
        _parseQueue.EnqueueTask(data);
    }

    private static NameIt GetNameIt(Message msg, IPEndPoint endPoint, DateTime Time)
    {
        Dictionary<string, string> headers = new();
        foreach (var h in msg.Sip.UnknownHeaders)
        {
            // todo check in db that we need this header
            var (key, value) = ParseUnknownHeader(h);
            headers[key] = value;
        }

        return new NameIt(
            headers,
            msg.Payload,
            msg.Sip.CallId,
            endPoint,
            Time);
    }

    private static (string, string) ParseUnknownHeader(string str)
    {
        var x = str.Split(':');
        x[0] = x[0].Replace("I-", "").Replace("X-", "");
        return (x[0].ToLower().Trim(), x[1].Trim());
    }
}
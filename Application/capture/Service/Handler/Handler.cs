using Capture.Service.Handler.provider;
using Capture.Service.Listener;
using Capture.Service.Parser;
using Capture.Service.TaskQueue;
using Database.Database;
using Database.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Handler;

public class Handler : IHandler, IDisposable
{
	private readonly ILogger<Handler> _logger;
	private readonly ICallsRepository _repository;
	private readonly TaskQueue<UdpReceiveResult> _parseQueue;
	private readonly BufferedTaskQueue<Data> _dbQueue;
	private readonly CustomBufferedQueue<string> _tmp;
	private readonly IOptionsProvider _provider;
	private readonly Timer _timer;
	private Stopwatch _stopwatch = new Stopwatch();

	private volatile int _counter = 0;
	private volatile bool _isWatched = false;

	private const int BufferSize = 1000;

	public Handler(ILogger<Handler> logger, ILogger<CustomBufferedQueue<string>> logger2, ICallsRepository repository, IOptionsProvider provider)
	{
		_logger = logger;
		_repository = repository;
		_parseQueue = new TaskQueue<UdpReceiveResult>(Parse, ParsingErrorHandler);
		_dbQueue = new BufferedTaskQueue<Data>(Save, bufferSize: BufferSize, exceptionHandler: SavingErrorHandler);
		_provider = provider;
		_timer = new Timer((e) => { _logger.LogDebug("Parser queue size {0}", _parseQueue.TaskCount); }, null,
			TimeSpan.Zero, TimeSpan.FromSeconds(60));

		_tmp = new CustomBufferedQueue<string>(_ => Task.CompletedTask, logger2);
	}

	private void Parse(UdpReceiveResult r)
	{
		var data = new ReceivedData(r.Buffer, r.RemoteEndPoint, DateTime.Now);
		var message = ParserHePv3.ParseMessage(data.Msg);
		var sipMethod = Enum.Parse<SipMethods>(message.Sip.CSeqMethod.ToString());
		if (!_provider.GetExcludedMethods().Contains(sipMethod))
		{
			var d = GetData(message, data.EndPoint, data.Time);
			_dbQueue.EnqueueTask(d);
		}
	}

	private void ParsingErrorHandler(Exception e, UdpReceiveResult y)
	{
		_logger.LogWarning("Error parsing message: {0}", e.Message);
	}

	private void SavingErrorHandler(Exception e, IList<Data> y)
	{
		_logger.LogWarning("Error saving message: {0}", e.Message);
	}

	private async Task Save(IList<Data> data)
	{
		// _stopwatch.Start();
		await _repository.InsertRangeAsync(data);
		// Interlocked.Add(ref _counter, BufferSize);
		// _logger.LogCritical(", {1}", _stopwatch.ElapsedMilliseconds / 1000.0);
	}

	public void HandleMessage(UdpReceiveResult r)
	{
		_parseQueue.EnqueueTask(r);
		_tmp.Push(r.RemoteEndPoint.Address.ToString());
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Database.Calls;

public class AvailableHeadersRepository: IAvailableHeadersRepository
{
    private ISet<string> _availableHeaders = new HashSet<string>();
    private readonly IContextFactory _contextFactory;
    private Timer _timer;
    
    public AvailableHeadersRepository(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer((e) =>
        {
            var nv = new HashSet<string>(FindAll());
            Interlocked.Exchange(ref _availableHeaders, nv);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));  
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _timer.Change(Timeout.Infinite, 0);
        _timer.Dispose();
        return Task.CompletedTask;
    }

    private string[] FindAll()
    {
        using var ctx = _contextFactory.CreateContext();
        return ctx.AvailableHeaders.Select(x => x.Header).ToArray();
    }

    public ISet<string> GetAvailableHeaders()
    {
        return _availableHeaders;
    }
}
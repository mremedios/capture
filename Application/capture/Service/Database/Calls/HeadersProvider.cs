using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Database.Calls;

public class HeadersProvider: IHeadersProvider
{
    private ISet<string> _availableHeaders = new HashSet<string>();
    private readonly IHeaderRepository _repo;
    private Timer _timer;
    
    public HeadersProvider(IHeaderRepository repository)
    {
        _repo = repository;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer((e) =>
        {
            var nv = new HashSet<string>(_repo.FindAvailableHeaders());
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

    public ISet<string> GetAvailableHeaders()
    {
        return _availableHeaders;
    }
}
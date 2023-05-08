using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Database;

namespace Capture.Service.Handler.provider;

public class OptionsProvider : IOptionsProvider
{
    private ISet<string> _availableHeaders = new HashSet<string>();
    private readonly IAvailableHeaderRepository _repo;
    private Timer _timer;

    public OptionsProvider(IAvailableHeaderRepository repository)
    {
        _repo = repository;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer((e) =>
        {
            var nv = new HashSet<string>(_repo.FindAll());
            Interlocked.Exchange(ref _availableHeaders, nv);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _timer.Change(Timeout.Infinite, 0);
        _timer.Dispose();
        return Task.CompletedTask;
    }

    public ISet<string> GetExcludedMethods()
    {
        return new HashSet<string> { "OPTIONS", "REGISTER" };
    }

    public ISet<string> GetAvailableHeaders()
    {
        return _availableHeaders;
    }
}
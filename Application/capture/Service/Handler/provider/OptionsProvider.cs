using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Database;
using Database.Models;

namespace Capture.Service.Handler.provider;

public class OptionsProvider : IOptionsProvider
{
    private ISet<string> _availableHeaders = new HashSet<string>();
    private ISet<SipMethods> _excludedMethods = new HashSet<SipMethods>();
    private readonly IAvailableHeaderRepository _repo;
    private readonly IMethodsRepository _methodsRepository;
    private Timer _timer;

    public OptionsProvider(IAvailableHeaderRepository repository, IMethodsRepository methodsRepository)
    {
        _repo = repository;
        _methodsRepository = methodsRepository;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer((e) =>
        {
            var nv = new HashSet<string>(_repo.FindAll());
            var methods = new HashSet<SipMethods>(_methodsRepository.FindAll());
            Interlocked.Exchange(ref _availableHeaders, nv);
            Interlocked.Exchange(ref _excludedMethods, methods);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _timer.Change(Timeout.Infinite, 0);
        _timer.Dispose();
        return Task.CompletedTask;
    }

    public ISet<SipMethods> GetExcludedMethods()
    {
        return _excludedMethods;
    }

    public ISet<string> GetAvailableHeaders()
    {
        return _availableHeaders;
    }
}
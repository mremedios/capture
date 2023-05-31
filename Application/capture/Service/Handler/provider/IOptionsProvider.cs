using Database.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Handler.provider;

public interface IOptionsProvider
{
	public ISet<string> GetAvailableHeaders();

	public ISet<SipMethods> GetExcludedMethods();

	public Task StartAsync(CancellationToken ct);

	public Task StopAsync(CancellationToken ct);
}
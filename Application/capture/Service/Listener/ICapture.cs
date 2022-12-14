using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service.Listener
{
	public interface ICapture : IDisposable
	{ 
		Task StartAsync(CancellationToken ct);
	}
}

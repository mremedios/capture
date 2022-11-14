using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capture.Service
{
	internal interface ICapture : IDisposable
	{
		Task StartAsync(CancellationToken ct);
	}
}

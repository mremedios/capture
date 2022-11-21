using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Capture.Service
{
	internal interface ICapture : IDisposable
	{
		Task StartAsync(CancellationToken ct);
	}
}

using System;
using Microsoft.Extensions.Hosting;

namespace Capture.Service
{
	internal interface ICapture : IHostedService, IDisposable
	{
		// Task StartAsync(CancellationToken ct);
	}
}

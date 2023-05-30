using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Capture.Service.Listener;

namespace Capture.Service.Handler;

public interface IHandler
{
    public void HandleMessage(ReceivedData data);

    public IEnumerable<Task> StartAsync(CancellationToken ct);
}
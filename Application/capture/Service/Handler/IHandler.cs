using System.Threading.Tasks;
using Capture.Service.Listener;

namespace Capture.Service.Handler;

public interface IHandler
{
    public void HandleMessage(ReceivedData data);

    public Task UpdateHeaders();
}
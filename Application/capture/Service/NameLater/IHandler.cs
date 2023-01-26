using Capture.Service.Listener;

namespace Capture.Service.NameLater;

public interface IHandler
{
    public void HandleMessage(ReceivedData data);
}
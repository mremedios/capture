using System.Net.Sockets;
using System.Threading.Tasks;
using Capture.Service.Listener;

namespace Capture.Service.Handler;

public interface IHandler
{
    public void HandleMessage(UdpReceiveResult data);
    
}
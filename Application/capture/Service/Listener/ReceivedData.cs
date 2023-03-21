using System;
using System.Net;

namespace Capture.Service.Listener;

public record ReceivedData(byte[] Msg, IPEndPoint EndPoint, DateTime Time);
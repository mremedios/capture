using System;
using System.Net;

namespace Capture.Service.Listener;

public record struct ReceivedData(byte[] Msg, IPEndPoint EndPoint, DateTime Time);
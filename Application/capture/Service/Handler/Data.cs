using System;
using System.Collections.Generic;
using System.Net;

namespace Capture.Service.Handler;

public record Data
(
    Dictionary<string, string> Headers,
    byte[] SipMessage,
    string CallId,
    IPEndPoint Host,
    DateTime Time
);
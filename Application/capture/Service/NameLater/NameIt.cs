using System;
using System.Collections.Generic;
using System.Net;

namespace Capture.Service.NameLater;

public record struct NameIt
(
    Dictionary<string, string> Headers,
    byte[] SipMessage,
    string CallId,
    IPEndPoint ServerIp,
    DateTime Time
);
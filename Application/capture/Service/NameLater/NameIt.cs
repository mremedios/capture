using System;
using System.Collections.Generic;
using System.Net;

namespace Capture.Service.NameLater;

public record NameIt
(
    Dictionary<string, string> Headers,
    byte[] SipMessage,
    string CallId,
    IPEndPoint Host,
    DateTime Time
);
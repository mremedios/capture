using System;
using System.Collections.Generic;
using System.Net;

namespace Capture.Service.Models;

public record Data
(
    Dictionary<string, string> Headers,
    string SipMessage,
    string CallId,
    IPEndPoint Host,
    DateTime ReceivingTime, 
    Details Details
);
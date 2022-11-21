using System;
using System.Net;

namespace Capture.Service
{
    public sealed class CaptureConfiguration
    {
        public string HomerAddress { get; set; }
        public int HomerPort { get; set; }
        public int Port { get; set; }
    }
}
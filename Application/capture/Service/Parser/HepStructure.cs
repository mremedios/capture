using System.Collections.Generic;
using System.Text;
using SIPSorcery.SIP;

namespace Capture.Service.Parser
{
    public class Message
    {
        public HEPStructure Hep;
        public SIPHeader Sip;
        public Dictionary<string, string> Headers;
        public byte[] Payload;
        
    }

    public class HEPStructure
    {
        public int ipFamily;
        public int protocolId;
        public ushort sourcePort;
        public ushort destinationPort;
        public uint timeSeconds;
        public uint timeUseconds;
        public int protocolType;
        public uint captureId;
        public string hepCorrelationID;
        public string captureAuthUser;
        public string sourceIPAddress;
        public string destinationIPAddress;
        public int uuid;
        public bool authorized;
        public string node;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HEPStructure[").Append("ipFamily: " + ipFamily).Append(", protcolId: " + protocolId)
                .Append(", sourcePort: " + sourcePort).Append(", destinationPort: " + destinationPort)
                .Append(", timeSeconds: " + timeSeconds).Append(", timeUseconds: " + timeUseconds)
                .Append(", protocolType: " + protocolType).Append(", captureId: " + captureId)
                .Append(", hepCorrelationID: " + hepCorrelationID).Append(", captureAuthUser: " + captureAuthUser)
                .Append(", sourceIPAddress: " + sourceIPAddress)
                .Append(", destinationIPAddress: " + destinationIPAddress).Append(", uuid: " + uuid)
                .Append(", authorized: " + authorized)
                .Append(", node: " + node).Append("]");
            return sb.ToString();
        }
    }
}
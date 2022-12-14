using System.Text;
using SIPSorcery.SIP;

namespace Capture.Service.Parser
{
    public class HEPStructure
    {
        public int ipFamily = 0;
        public int protocolId = 0;
        public ushort sourcePort = 0;
        public ushort destinationPort = 0;
        public uint timeSeconds = 0;
        public uint timeUseconds = 0;
        public int protocolType = 0;
        public uint captureId = 0;
        public string hepCorrelationID = null;
        public string captureAuthUser = null;
        public string sourceIPAddress = null;
        public string destinationIPAddress = null;
        public int uuid = 0;
        public bool authorized = false;
        public byte[] payloadByteMessage = null;
        public SIPHeader headers = null;
        public long recievedTimestamp;
        public string node;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var payload = Encoding.UTF8.GetString(payloadByteMessage);
            sb.Append("HEPStructure[").Append("ipFamily: " + ipFamily).Append(", protcolId: " + protocolId)
                .Append(", sourcePort: " + sourcePort).Append(", destinationPort: " + destinationPort)
                .Append(", timeSeconds: " + timeSeconds).Append(", timeUseconds: " + timeUseconds)
                .Append(", protocolType: " + protocolType).Append(", captureId: " + captureId)
                .Append(", hepCorrelationID: " + hepCorrelationID).Append(", captureAuthUser: " + captureAuthUser)
                .Append(", sourceIPAddress: " + sourceIPAddress)
                .Append(", destinationIPAddress: " + destinationIPAddress).Append(", uuid: " + uuid)
                .Append(", authorized: " + authorized).Append(", payloadByteMessage: " + payload)
                .Append(", recievedTimestamp: " + recievedTimestamp).Append(", node: " + node).Append("]")
                .Append("Headers:" + headers.ToString());
            return sb.ToString();
        }
    }
}
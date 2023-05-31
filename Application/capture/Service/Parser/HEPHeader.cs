using SIPSorcery.SIP;

namespace Capture.Service.Parser
{
	public class Message
	{
		public HEPHeader Hep;
		public SIPHeader Sip;
		public byte[] Payload;
	}

	public class HEPHeader
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
	}
}
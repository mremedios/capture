using Extension;
using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Service.Parser
{
	public class ParserHePv3
	{
		private const int Offset = 6;

		private readonly BinaryReader _reader;
		private readonly MemoryStream _data;


		public ParserHePv3(Stream input)
		{
			_reader = new BinaryReader(input);
		}
		public ParserHePv3()
		{
			_data = new MemoryStream(1024);
			_reader = new BinaryReader(_data);
		}

		public ValueTask Enqueue(byte[] bytes)
		{
			if (_data == null)
			{
				throw new NotSupportedException();
			}

			return _data.WriteAsync(bytes);
		}

		public void Parse()
		{
			while (true)
			{
				var hep = ParseMessage();
				Console.WriteLine(hep.headers.CallId);
				// Console.WriteLine("id: {0}, source: {1}", hep.captureId, hep.sourceIPAddress);
			}
		}
		public HEPStructure ParseMessage()
		{
			var header = _reader.ReadBytes(4);
			var expected = new byte[] { 0x48, 0x45, 0x50, 0x33 };
			if (!header.SequenceEqual(expected)) throw new IOException("Invalid HEP header."); //  ¯\_(ツ)_/¯
			var length = _reader.ReadInt16Be();
			var hep = Parse(length);

			var payload = SIPSorcery.SIP.SIPMessageBuffer.ParseSIPMessage(hep.payloadByteMessage, null, null);
			hep.headers = SIPSorcery.SIP.SIPHeader.ParseSIPHeaders(payload.SIPHeaders);
			if (hep.headers.ContentLength > 0)
			{

			}
			return hep;
		}

		private HEPStructure Parse(int totalLength)
		{
			HEPStructure hepStruct = new HEPStructure();
			try
			{
				int i = Offset;
				hepStruct.recievedTimestamp = hepStruct.timeSeconds * 1000000 + hepStruct.timeUseconds;
				while (i < totalLength)
				{
					var chunkId = _reader.ReadInt16Be();
					var chunkType = _reader.ReadInt16Be();
					var chunkLength = _reader.ReadInt16Be();

					if (chunkLength > totalLength)
					{
						Console.WriteLine("Corrupted HEP: CHUNK LENGHT couldn't be bigger as CHUNK_LENGHT");
						_reader.ReadBytes(totalLength - i);
						throw new IOException("Invalid HEP payload.");
					}

					if (chunkLength == 0)
					{
						Console.WriteLine("Corrupted HEP: LENGTH couldn't be 0!");
						continue;
					}

					if (chunkId != 0)
					{
						_reader.ReadBytes(chunkLength - Offset); // skip chunk
						continue;
					}

					switch (chunkType)
					{
						case 1:
							hepStruct.ipFamily = _reader.ReadByte();
							break;
						case 2:
							hepStruct.protocolId = _reader.ReadByte();
							break;
						case 3:
							var src_ip4 = _reader.ReadBytes(4);
							hepStruct.sourceIPAddress = IpToString(src_ip4);
							break;
						case 4:
							byte[] dst_ip4 = _reader.ReadBytes(4);
							hepStruct.destinationIPAddress = IpToString(dst_ip4);
							break;
						// ipv6 не нужен :)
						case 7:
							hepStruct.sourcePort = _reader.ReadUInt16Be();
							break;
						case 8:
							hepStruct.destinationPort = _reader.ReadUInt16Be();
							break;
						case 9:
							hepStruct.timeSeconds = _reader.ReadUInt32Be();
							break;
						case 10:
							hepStruct.timeUseconds = _reader.ReadUInt32Be();
							break;
						case 11:
							hepStruct.protocolType = _reader.ReadByte();
							break;
						case 12:
							hepStruct.captureId = _reader.ReadUInt32Be();
							break;
						case 14:
							var key = _reader.ReadBytes(chunkLength - Offset);
							hepStruct.captureAuthUser = Encoding.UTF8.GetString(key);
							break;
						case 15:
							hepStruct.payloadByteMessage = _reader.ReadBytes(chunkLength - Offset);
							break;
						case 16: // compressed payload
							var input = _reader.ReadBytes(chunkLength - Offset);
							hepStruct.payloadByteMessage = ExtractBytes(input);
							break;
						case 17:
							hepStruct.hepCorrelationID =
								Encoding.UTF8.GetString(_reader.ReadBytes(chunkLength - Offset));
							break;
						default:
							Console.WriteLine("Unknown default chunk: [" + chunkType + "]");
							_reader.ReadBytes(chunkLength - Offset); // skip chunk
							break;
					}

					i += chunkLength;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Console.WriteLine("Unable RUN WORKER");
				throw;
			}

			return hepStruct;
		}

		private static byte[] ExtractBytes(byte[] input)
		{
			MemoryStream msInner = new MemoryStream();
			MemoryStream ms = new MemoryStream(input);

			ms.Seek(2, SeekOrigin.Begin);
			using (DeflateStream z = new DeflateStream(ms, CompressionMode.Decompress))
			{
				z.CopyTo(msInner);
			}

			return msInner.ToArray();
		}

		private static String IpToString(byte[] address)
		{
			return string.Join(".", address);
		}
	}
}

namespace Extension
{
	static class BinaryReaderExtension
	{
		public static int ReadInt16Be(this BinaryReader r)
		{
			return BinaryPrimitives.ReverseEndianness(r.ReadInt16());
		}

		public static ushort ReadUInt16Be(this BinaryReader r)
		{
			return BinaryPrimitives.ReverseEndianness(r.ReadUInt16());
		}

		public static uint ReadUInt32Be(this BinaryReader r)
		{
			return BinaryPrimitives.ReverseEndianness(r.ReadUInt32());
		}
	}
}
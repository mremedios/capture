using System.Buffers.Binary;
using System.IO;

namespace Capture.Service.Parser
{
	public static class BinaryReaderExtension
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
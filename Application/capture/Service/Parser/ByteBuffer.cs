using System;
using System.Buffers.Binary;
using System.IO;

namespace Capture.Service.Parser;

/*
 * Можно эту идею использовать вместо BinaryReader
 */
public static class ByteBuffer
{
    public static int ReadInt16Be(this Span<byte> r, int start)
    {
        return BinaryPrimitives.ReadInt16BigEndian(r.Slice(start, 2));
    }
    
    public static ushort ReadUInt16Be(this Span<byte> r, int start)
    {
        return BinaryPrimitives.ReadUInt16BigEndian(r.Slice(start, 2));
    }
    
    public static uint ReadUInt32Be(this Span<byte> r, int start)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(r.Slice(start, 2));
    }
    
    public static uint ReadBytes(this Span<byte> r, int start)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(r.Slice(start, 2));
    }
    
}
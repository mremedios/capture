using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Linq;

namespace Capture.Service.Parser
{
    public static class ParserHePv3
    {
        private const int Offset = 6;
        public static Message ParseMessage(byte[] msg)
        {
            var reader = new BinaryReader(new MemoryStream(msg));

            var header = reader.ReadBytes(4);
            var expected = new byte[] { 0x48, 0x45, 0x50, 0x33 };
            if (!header.SequenceEqual(expected)) throw new IOException("Invalid HEP header.");
            var length = reader.ReadInt16Be();
            var message = Parse(reader, length);

            var payload = SIPSorcery.SIP.SIPMessageBuffer.ParseSIPMessage(message.Payload, null, null);
            message.Sip = SIPSorcery.SIP.SIPHeader.ParseSIPHeaders(payload.SIPHeaders);
            
            return message;
        }

        private static Message Parse(BinaryReader reader, int totalLength)
        {
            var hepStruct = new HEPHeader();
            var message = new Message();
            try
            {
                int i = Offset;
                while (i < totalLength)
                {
                    var chunkId = reader.ReadInt16Be();
                    var chunkType = reader.ReadInt16Be();
                    var chunkLength = reader.ReadInt16Be();

                    if (chunkLength > totalLength)
                    {
                        reader.ReadBytes(totalLength - i);
                        throw new IOException( // todo throw custom exception?
                            "Invalid HEP payload. Corrupted HEP: CHUNK LENGHT couldn't be bigger as CHUNK_LENGHT");
                    }

                    if (chunkLength == 0)
                    {
                        Console.WriteLine("Corrupted HEP: LENGTH couldn't be 0!"); // todo
                        continue;
                    }

                    if (chunkId != 0)
                    {
                        reader.ReadBytes(chunkLength - Offset); // skip chunk
                        continue;
                    }

                    switch (chunkType)
                    {
                        case 1:
                            hepStruct.ipFamily = reader.ReadByte();
                            break;
                        case 2:
                            hepStruct.protocolId = reader.ReadByte();
                            break;
                        case 3:
                            var srcIp4 = reader.ReadBytes(4);
                            hepStruct.sourceIPAddress = IpToString(srcIp4);
                            break;
                        case 4:
                            var dstIp4 = reader.ReadBytes(4);
                            // TODO: положить в базу байты, а не строку
                            hepStruct.destinationIPAddress = IpToString(dstIp4);
                            break;
                        case 7:
                            hepStruct.sourcePort = reader.ReadUInt16Be();
                            break;
                        case 8:
                            hepStruct.destinationPort = reader.ReadUInt16Be();
                            break;
                        case 9:
                            hepStruct.timeSeconds = reader.ReadUInt32Be();
                            break;
                        case 10:
                            hepStruct.timeUseconds = reader.ReadUInt32Be();
                            break;
                        case 11:
                            hepStruct.protocolType = reader.ReadByte();
                            break;
                        case 12:
                            hepStruct.captureId = reader.ReadUInt32Be();
                            break;
                        case 14:
                            var key = reader.ReadBytes(chunkLength - Offset);
                            hepStruct.captureAuthUser = Encoding.UTF8.GetString(key);
                            break;
                        case 15:
                            message.Payload = reader.ReadBytes(chunkLength - Offset);
                            break;
                        case 16: // compressed payload
                            var input = reader.ReadBytes(chunkLength - Offset);
                            message.Payload = ExtractBytes(input);
                            break;
                        case 17:
                            hepStruct.hepCorrelationID =
                                Encoding.UTF8.GetString(reader.ReadBytes(chunkLength - Offset));
                            break;
                        default:
                            Console.WriteLine("Unknown default chunk: [" + chunkType + "]"); //todo
                            reader.ReadBytes(chunkLength - Offset); // skip chunk
                            break;
                    }

                    i += chunkLength;
                }
            }
            catch (Exception e)
            {
                throw new IOException("Invalid HEP payload. " + e.Message); 
            }

            message.Hep = hepStruct;
            return message;
        }

        private static byte[] ExtractBytes(byte[] input) // todo check it
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

        private static string IpToString(byte[] address)
        {
            return string.Join(".", address);
        }
    }
}
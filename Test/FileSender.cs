using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Test;

public class FileSender
{
    private static void GetPackages(Stream input, ISender sender)
    {
        try
        {
            var buffer = new byte[16383];
            var last = string.Empty;
            int read;
            while ((read = input.Read(buffer)) > 0)
            {
                var str = BitConverter.ToString(buffer, 0, read);
                var packages = Regex.Split(last + str, @"(?=48-45-50-33)");
                last = packages.Last();
                last = last.Length == 0? last : last + "-";
                foreach (var package in packages.SkipLast(1))
                {
                    if (package.Length != 0)
                    {
                        var s = package.Trim();
                        var byteStr = s.EndsWith('-') ? s.Remove(s.Length - 1) : s;
                        var byteArr = byteStr
                            .Split('-')
                            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                            .ToArray();

                        sender.Send(byteArr);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [Test]
    public void Start()
    {
        using Stream input = File.OpenRead("input/HEP_sample_20221202_103917.bin");
        GetPackages(input, new UdpSender());
    }   
}

internal interface ISender
{
    public void Send(byte[] arr);
}

internal class UdpSender : ISender
{
    private UdpClient client;

    public UdpSender()
    {
        client = new UdpClient();
        client.Connect(IPAddress.Loopback, 9060);
    }

    public void Send(byte[] arr)
    {
        try
        {
            
            client.Send(arr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using WebSocketSharp;

namespace Test;

public class Sender
{
    private static void GetPackages(Stream input, Action<string> f)
    {
        try
        {
            var buffer = new byte[32768];
            var last = string.Empty;
            int read;
            while ((read = input.Read(buffer)) > 0)
            {
                var str = BitConverter.ToString(buffer, 0, read);
                var packages = Regex.Split(last + str, @"(?=48-45-50-33)");
                last = packages.Last();
                foreach (var package in packages.SkipLast(1))
                {
                    if (!package.IsNullOrEmpty()) f(package.Trim());
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void Start()
    {
        var client = new UdpClient();
        client.Connect(IPAddress.Loopback, 9060);

        using (Stream input = File.OpenRead("input/HEP_sample_20221202_103917.bin"))
        {
            void Send(string s)
            {

                var byteStr = s.EndsWith('-') ? s.Remove(s.Length - 1) : s;
                try {
                    client.Send(
                        byteStr.Split('-')
                            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                            .ToArray()
                    );
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            GetPackages(input, Send);
        }
    }
}
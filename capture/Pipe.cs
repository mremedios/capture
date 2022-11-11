using System.Net;
using System.Net.Sockets;

var sendEndpoint = new IPEndPoint(IPAddress.Loopback, 9999);
var listenEndpoint = new IPEndPoint(IPAddress.Any, 8888);
const string outputFileName = "Output";

try
{
    using TcpClient sender = new();
    await sender.ConnectAsync(sendEndpoint);
    var outputStream = sender.GetStream();

    var fileWriter = new BinaryWriter(File.OpenWrite(outputFileName));

    var listener = new TcpListener(listenEndpoint);
    listener.Start();
    var client = await listener.AcceptTcpClientAsync();

    var buffer = new Byte[1024];

    while (true)
    {
        await using var inputStream = client.GetStream();
        int length;

        while ((length = inputStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            fileWriter.Write(buffer, 0, length);
            outputStream.Write(buffer, 0, length);
        }

        fileWriter.Flush();
        fileWriter.Close();
    }
}
catch (SocketException socketException)
{
    Console.Write(socketException.Message);
}
catch (SystemException)
{
}
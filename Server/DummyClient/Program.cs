using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var hostName = Dns.GetHostName();
        var ipHostEntry = Dns.GetHostEntry(hostName);
        var remoteEp = new IPEndPoint(ipHostEntry.AddressList[0], 11000);

        var socket = new Socket(remoteEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(remoteEp);

            Console.WriteLine($"Connect to {remoteEp.ToString()}");

            var sendBuffer = Encoding.UTF8.GetBytes("hello! 안녕! 반가워~");
            var sendBytes = socket.Send(sendBuffer);

            var recvBuffer = new byte[1024];
            var recvBytes = socket.Receive(recvBuffer);

            var recvString = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
            Console.WriteLine($"[From Server] : {recvString}");

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
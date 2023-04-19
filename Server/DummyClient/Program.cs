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

        for (var count = 0; count < 1000; count++)
        {
            var socket = new Socket(remoteEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(remoteEp);

                Console.WriteLine($"Client:{count} Connect to {remoteEp.ToString()}");

                for (var i = 0; i < 5; i++)
                {
                    var sendBuffer = Encoding.UTF8.GetBytes($"hello! 안녕! 반가워~ {i}");
                    var sendBytes = socket.Send(sendBuffer);
                }

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

            Thread.Sleep(100);
        }
    }
}
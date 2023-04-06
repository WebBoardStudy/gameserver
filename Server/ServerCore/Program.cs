using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

internal class Program
{
    private static Listener _listener = new();

    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint);

        while (true)
        {
            Console.WriteLine("Listening...");

            var clientSocket = _listener.Accept();

            try
            {
                var recvBuffer = new byte[1024];
                var recvBytes = clientSocket.Receive(recvBuffer);
                var recvString = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);

                Console.WriteLine($"[From Client] : {recvString}");

                var sendBuffer =
                    Encoding.UTF8.GetBytes($"Welcome to Kauri Server! You send to me this message :{recvString}");

                clientSocket.Send(sendBuffer);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
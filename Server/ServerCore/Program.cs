using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

internal class Program
{
    private static Listener _listener = new();

    private static void OnAcceptHandler(Socket clientSocket)
    {
        try
        {
            var session = new Session();
            session.Start(clientSocket);

            var sendBuffer =
                Encoding.UTF8.GetBytes($"Welcome to Kauri Server");
            session.Send(sendBuffer);

            Thread.Sleep(1000);

            session.Disconnect();
            session.Disconnect();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint, OnAcceptHandler);
        Console.WriteLine("Listening...");

        while (true) ;
    }
}
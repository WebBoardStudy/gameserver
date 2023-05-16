using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server;

internal class Program
{
    private class GameSession : Session
    {
        public override void OnConnected(EndPoint? endPoint)
        {
            try
            {
                Console.WriteLine($"OnConnected : {endPoint}");


                var sendBuffer =
                    Encoding.UTF8.GetBytes($"Welcome to Kauri Server");
                Send(sendBuffer);

                Thread.Sleep(1000);

                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnDisconnected(EndPoint? endPoint)
        {
            try
            {
                Console.WriteLine($"onDisconnect : {endPoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnDisconnected failed : {ex}");
            }
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            try
            {
                Debug.Assert(buffer.Array != null, "buffer.Array != null");
                var recvStr = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Client] {recvStr}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnRecv failed : {ex}");
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send To Client, Transfered bytes: {numOfBytes}");
        }
    }

    private static Listener _listener = new();

    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint, () => new GameSession());
        Console.WriteLine("Listening...");

        while (true) ;
    }
}
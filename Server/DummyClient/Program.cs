using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

internal class Program
{
    private class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint) {
            try
            {
                Console.WriteLine($"OnConnected : {endPoint}");

                for (var i = 0; i < 5; i++)
                {
                    var sendBuffer = Encoding.UTF8.GetBytes($"hello! 안녕! 반가워~ {i}");
                    Send(sendBuffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
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

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            try
            {
                Debug.Assert(buffer.Array != null, "buffer.Array != null");
                var recvStr = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Server] {recvStr}");
                return buffer.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnRecv failed : {ex}");
                return 0;
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send To Server! Transfered bytes: {numOfBytes}");
        }
    }

    private static void Main(string[] args)
    {
        var hostName = Dns.GetHostName();
        var ipHostEntry = Dns.GetHostEntry(hostName);
        var remoteEp = new IPEndPoint(ipHostEntry.AddressList[0], 11000);

        try {
            var connector = new Connector();
            connector.Connect(remoteEp, () => new GameSession());
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }

        while (true) {
           
        }
        
    }
}
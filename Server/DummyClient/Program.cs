using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

internal class Program
{
    class Packet {
        public ushort size;
        public ushort packetId;
    }


    private class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint) {
            try
            {
                Console.WriteLine($"OnConnected : {endPoint}");


                Packet packet = new Packet() { size = 4, packetId = 10 };

                for (var i = 0; i < 5; i++)
                {
                    ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                    byte[] buf = BitConverter.GetBytes(packet.size);
                    byte[] buf2 = BitConverter.GetBytes(packet.packetId);
                    Array.Copy(buf, 0, openSegment.Array, openSegment.Offset, buf.Length);
                    Array.Copy(buf2, 0, openSegment.Array, openSegment.Offset + buf.Length, buf2.Length);
                    ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);

                    Send(sendBuff);
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
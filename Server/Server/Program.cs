using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server;

internal class Program
{
    class Packet {
        public ushort size;
        public ushort packetId;
    }
   
    private class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");

                //Packet packet = new Packet() { size = 4, packetId = 10 };
              
                //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                //byte[] buf = BitConverter.GetBytes(packet.size);
                //byte[] buf2 = BitConverter.GetBytes(packet.packetId);
                //Array.Copy(buf, 0, openSegment.Array, openSegment.Offset, buf.Length);
                //Array.Copy(buf2, 0, openSegment.Array, openSegment.Offset + buf.Length, buf2.Length);
                //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buf.Length + buf2.Length);
                
                //Send(sendBuff);

                Thread.Sleep(10000);

                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + sizeof(ushort));

            Console.WriteLine($"OnRecvPacket, size: {size} packetId:{packetId}");
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
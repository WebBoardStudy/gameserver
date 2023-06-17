using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server;

internal class Program
{
    class Knight {
        public int hp;
        public int attack;
        public string name;
        public List<int> skills = new List<int>();
    }

    private class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");


                Knight knight = new Knight() { hp = 100, attack = 10 };

                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                byte[] buf = BitConverter.GetBytes(knight.hp);
                byte[] buf2 = BitConverter.GetBytes(knight.attack);
                Array.Copy(buf, 0, openSegment.Array, openSegment.Offset, buf.Length);
                Array.Copy(buf2, 0, openSegment.Array, openSegment.Offset + buf.Length, buf2.Length);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(buf.Length + buf2.Length);
                
                Send(sendBuff);

                Thread.Sleep(1000);

                Disconnect();
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
                Console.WriteLine($"[From Client] {recvStr}");
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
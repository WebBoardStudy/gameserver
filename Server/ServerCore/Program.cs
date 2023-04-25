using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ServerCore;

internal class Program {
    class GameSession : Session {
        public override void OnConnectd(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnectd : {endPoint}");


                var sendBuffer =
                    Encoding.UTF8.GetBytes($"Welcome to Kauri Server");
                Send(sendBuffer);

                Thread.Sleep(1000);

                Disconnect();
            }
            catch (Exception ex) {
                Console.WriteLine($"OnConnectd failed : {ex}");
            }
        }

        public override void OnDisconnected(EndPoint? endPoint) {
            try {
                Console.WriteLine($"onDisconnect : {endPoint}");
            }
            catch (Exception ex) {
                Console.WriteLine($"OnDisconnected failed : {ex}");
            }
        }

        public override void OnRecv(ArraySegment<byte> buffer) {
            try {
                Debug.Assert(buffer.Array != null, "buffer.Array != null");
                var recvStr = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Client] {recvStr}");
            }
            catch (Exception ex) {
                Console.WriteLine($"OnRecv failed : {ex}");
            }
        }

        public override void OnSend(int numOfBytes) {
            Console.WriteLine($"Transfered bytes: {numOfBytes}");
        }
    }

    private static Listener _listener = new();

    private static void Main(string[] args) {
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint, () => { return new GameSession(); });
        Console.WriteLine("Listening...");

        while (true) ;
    }
}
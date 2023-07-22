using ServerCore;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace DummyClient {
    
    class ServerSession : PacketSession {
        public override void OnConnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");
            }
            catch (Exception ex) {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnDisconnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"onDisconnect : {endPoint}");
            }
            catch (Exception ex) {
                Console.WriteLine($"OnDisconnected failed : {ex}");
            }
        }
        

        public override void OnSend(int numOfBytes) {
            //Console.WriteLine($"Send To Server! Transfered bytes: {numOfBytes}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
    }
}

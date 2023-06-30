using ServerCore;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace DummyClient {
    
    class ServerSession : Session {
        public override void OnConnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");

                PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "테스트플레이어" };
                packet.skills.Add(new PlayerInfoReq.Skill() { id = 1, level = 2, duration = 3.3f });
                packet.skills.Add(new PlayerInfoReq.Skill() { id = 2, level = 13, duration = 123.9f });

                ArraySegment<byte> sendBuff = packet.Write();
                Send(sendBuff);
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

        public override int OnRecv(ArraySegment<byte> buffer) {
            try {
                Debug.Assert(buffer.Array != null, "buffer.Array != null");
                var recvStr = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Server] {recvStr}");
                return buffer.Count;
            }
            catch (Exception ex) {
                Console.WriteLine($"OnRecv failed : {ex}");
                return 0;
            }
        }

        public override void OnSend(int numOfBytes) {
            Console.WriteLine($"Send To Server! Transfered bytes: {numOfBytes}");
        }
    }
}

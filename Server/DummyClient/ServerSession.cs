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

                C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "테스트플레이어" };
                packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 1, level = 2, duration = 3.3f });
                packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 2, level = 13, duration = 123.9f });

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
        

        public override void OnSend(int numOfBytes) {
            Console.WriteLine($"Send To Server! Transfered bytes: {numOfBytes}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
    }
}

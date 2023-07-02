using Server;
using ServerCore;
using System.Net;
using System.Text;

namespace Server {
  
    class ClientSession : PacketSession {
        public override void OnConnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");
                                
                S_Test s_Test = new S_Test();
                s_Test.name = "테스트 서버에 잘 오셨습니다.";
                s_Test.testFloat = 1235678.12f;
                var sb = s_Test.Write();
                Send(sb);

                Thread.Sleep(10000);

                Disconnect();
            }
            catch (Exception ex) {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) {
            PacketManager.Instance.OnRecvPacket(this, buffer);
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
            Console.WriteLine($"Send To Client, Transfered bytes: {numOfBytes}");
        }
    }
}

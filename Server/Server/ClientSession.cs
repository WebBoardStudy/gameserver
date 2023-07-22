using Server;
using ServerCore;
using System.Net;
using System.Text;

namespace Server {
  
    class ClientSession : PacketSession {
        public int SessionID {  get; set; }
        public GameRoom Room { get; set; }


        public override void OnConnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");

                Program.gameRoom.Push(() => Program.gameRoom.Enter(this));
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
                SessionManager.Instance.Remove(this);
                if (Room != null ) {
                    GameRoom room = Room;
                    Room.Push(() => room.Leave(this));
                    Room = null;
                }
                Console.WriteLine($"onDisconnect : {endPoint}");
            }
            catch (Exception ex) {
                Console.WriteLine($"OnDisconnected failed : {ex}");
            }
        }

        public override void OnSend(int numOfBytes) {
            //Console.WriteLine($"Send To Client, Transfered bytes: {numOfBytes}");
        }
    }
}

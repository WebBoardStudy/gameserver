using Server;
using ServerCore;
using System.Net;
using System.Text;

namespace Server {
  
    class ClientSession : PacketSession {
        public override void OnConnected(EndPoint endPoint) {
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
            catch (Exception ex) {
                Console.WriteLine($"OnConnected failed : {ex}");
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer) {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)packetId) {
                case PacketID.PlayerInfoReq: {
                        PlayerInfoReq packet = new PlayerInfoReq();
                        packet.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: playerId:{packet.playerId} name:{packet.name}");
                        foreach (var skill in packet.skills) {
                            Console.WriteLine($"\tSkill({skill.id} {skill.level} {skill.duration})");
                        }
                    }
                    break;
            }

            Console.WriteLine($"OnRecvPacket, packetId:{packetId} size: {size} ");
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

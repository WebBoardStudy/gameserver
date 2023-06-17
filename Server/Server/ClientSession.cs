using ServerCore;
using System.Net;

namespace Server {
    abstract class Packet {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet {
        public long playerId;

        public PlayerInfoReq() {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s) {
            ushort count = 0;
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            //ushort packetId = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;

            this.playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            count += 8;
        }
        public override ArraySegment<byte> Write() {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            count += 2;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;

            // write packet size
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (!success) {
                return null;
            }

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID {
        PlayerInfoReq = 1,
        PlayerInfoRes = 2,
    }



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

            switch((PacketID)packetId) {
                case PacketID.PlayerInfoReq: {                        
                        PlayerInfoReq packet = new PlayerInfoReq();
                        packet.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: {packet.playerId}");
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

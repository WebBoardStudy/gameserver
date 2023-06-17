using ServerCore;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace DummyClient {
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

    class ServerSession : Session {
        public override void OnConnected(EndPoint endPoint) {
            try {
                Console.WriteLine($"OnConnected : {endPoint}");

                PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };
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

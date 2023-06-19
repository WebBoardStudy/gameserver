﻿using ServerCore;
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
        public string name;
        public List<SkillInfo> skillInfos = new List<SkillInfo>();

        public struct SkillInfo {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count) {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
                count += sizeof(float);
                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count) {
                this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);

                this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);

                this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public PlayerInfoReq() {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment) {
            if (segment == null) {
                return;
            }

            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += sizeof(ushort);
            //ushort packetId = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(ushort);

            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            skillInfos.Clear();

            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            
            for(ushort i = 0; i < skillLen; i++) {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.Read(s, ref count);
                this.skillInfos.Add(skillInfo);
            }

        }
        public override ArraySegment<byte> Write() {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);
                        
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length,  segment.Array, segment.Offset + count + sizeof(ushort));

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);

            count += sizeof(ushort);
            count += nameLen;

            //skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skillInfos.Count);
            count += sizeof(ushort);
            
            foreach (var skillInfo in this.skillInfos) {
                success &= skillInfo.Write(s, ref count);
            }
            
            // write packet size
            success &= BitConverter.TryWriteBytes(s, count);

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

                PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "테스트플레이어" };
                packet.skillInfos.Add(new PlayerInfoReq.SkillInfo() { id = 1, level = 2, duration = 3.3f });
                packet.skillInfos.Add(new PlayerInfoReq.SkillInfo() { id = 2, level = 13, duration = 123.9f });

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

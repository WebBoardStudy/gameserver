using System;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.data;
using Server.Game;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public Player MyPlaeyr { get; set; }
        public int SessionId { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // PROTO Test
            MyPlaeyr = ObjectMansger.Instance.Add<Player>();
            {
                MyPlaeyr.Info.Name = $"Player_{MyPlaeyr.Info.ObjectId}";
                MyPlaeyr.Info.PosInfo.State = CreatureState.Idle;
                MyPlaeyr.Info.PosInfo.MoveDir = MoveDir.Down;
                MyPlaeyr.Info.PosInfo.PosX = 0;
                MyPlaeyr.Info.PosInfo.PosY = 0;
                
                StatInfo stat = null;
                DataManager.StatDict.TryGetValue(1, out stat);
                MyPlaeyr.Stat.MergeFrom(stat);
            
                MyPlaeyr.Session = this;
            }
            
            RoomManager.Instance.Find(1).EnterGame(MyPlaeyr);
        }

        public void Send(IMessage paket)
        {
            String msgName = paket.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            ushort size = (ushort)paket.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(paket.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            RoomManager.Instance.Find(1).LeaveGame(MyPlaeyr.Info.ObjectId);
            
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
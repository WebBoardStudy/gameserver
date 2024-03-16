using System;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
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
            MyPlaeyr = PlayerManager.Instance.Add();
            {
                MyPlaeyr.info.Name = $"Player_{MyPlaeyr.info.PlayerId}";
                MyPlaeyr.info.PosInfo.State = CreatureState.Idle;
                MyPlaeyr.info.PosInfo.MoveDir = MoveDir.None;
                MyPlaeyr.info.PosInfo.PosX = 0;
                MyPlaeyr.info.PosInfo.PosY = 0;
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
            RoomManager.Instance.Find(1).LeaveGame(MyPlaeyr.info.PlayerId);
            
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
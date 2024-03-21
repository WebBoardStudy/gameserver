using Protocol;
using System;

namespace Server.Game.Object
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        long _nextUpdateTick = 0;

        public override void Update()
        {
            if (Owner == null || Room == null)
                return;

            if (_nextUpdateTick >= Environment.TickCount64)
                return;

            _nextUpdateTick = Environment.TickCount64 + 50;

            var destPos = GetFrontCellPos();
            if (Room.Map.CanGo(destPos))
            {
                CellPos = destPos;
                var movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(movePacket);

                Console.WriteLine($"Move Arrow, Id:{Id}");
            }
            else
            {
                var target = Room.Map.Find(destPos);
                if (target != null)
                {
                    // TODO : 피격 판정
                }

                Room.LeaveGame(Id);
            }
        }
    }
}

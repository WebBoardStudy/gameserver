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
            if (Owner == null || Room == null || Data == null || Data.projectile == null)
                return;

            if (_nextUpdateTick >= Environment.TickCount64)
                return;

            long tick = (long)(1000 / Speed);
            _nextUpdateTick = Environment.TickCount64 + tick;

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
                    target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
                }

                Room.LeaveGame(Id);
            }
        }
    }
}

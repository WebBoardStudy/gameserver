using System;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class Arrow : Projectile
{
    public GameObject Owner { get; set; }
    
    long _nextMoveTick = 0;
    
    public override void Update()
    {
        if (Owner == null || Data.projectile == null || Room == null)
            return;

        if (_nextMoveTick >= Environment.TickCount64)
            return;

        long tick = (long) (1000 / Data.projectile.speed);
        _nextMoveTick = Environment.TickCount64 + tick;

        Vector2Int destPos = GetFrontCellPos();
        if (Room.Map.CanGo(destPos))
        {
            CellPos = destPos;
            
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);

            Console.WriteLine("Move Arrow");
        }
        else
        {
            GameObject target = Room.Map.Find(destPos);
            if (target != null)
            {
                target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
            }
            
            // 소멸
            Room.LeaveGame(Id);
        }
    }
}
using System;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class Player : GameObject
{
    public ClientSession Session { get; set; }

    public Player()
    {
        ObjectType = GameObjectType.Player;
        Speed = 20.0f;
    }

    public override void OnDamaged(GameObject attcker, int damage)
    {
        base.OnDamaged(attcker, damage);
    }

    public override void OnDead(GameObject attacker)
    {
        base.OnDead(attacker);
    }
}
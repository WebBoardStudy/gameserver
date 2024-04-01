using Protocol;
using System;

namespace Server.Game.Object;

public class Player : GameObject
{
    public ClientSession Session { get; set; }

    public Player()
    {
        ObjectType = GameObjectType.Player;
        Speed = 20.0f;
    }

    public override void OnDamaged(GameObject attacker, int damage)
    {
        Console.WriteLine($"TODO: damage {damage}");
    }
}
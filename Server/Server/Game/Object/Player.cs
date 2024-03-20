using Protocol;
using Server.Game.Room;

namespace Server.Game.Object;

public class Player : GameObject
{
    public ClientSession Session { get; set; }
    
    public Player()
    {
        ObjectType = GameObjectType.Player;
    }
}
using Protocol;

namespace Server.Game.Object
{
    public class Projectile : GameObject
    {
        public Data.Skill Data { get; set; }
        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public virtual void Update()
        {

        }
    }
}

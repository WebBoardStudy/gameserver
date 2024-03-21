using Google.Protobuf;
using Protocol;
using Server.Game.Object;
using System;
using System.Collections.Generic;

namespace Server.Game.Room;

public class GameRoom
{
    object _lock = new object();
    public int RoomId { get; set; }

    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
    Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

    public Map Map { get; private set; } = new Map();

    public void Init(int mapId)
    {
        Map.LoadMap(mapId);
    }

    public void EnterGame(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }

        var type = ObjectManager.GetObjectTypeById(gameObject.Id);
        lock (_lock)
        {
            gameObject.Room = this;

            switch (type)
            {
                case GameObjectType.Player:
                    var newPlayer = gameObject as Player;
                    _players.Add(gameObject.Info.ObjectId, gameObject as Player);

                    // 본인한테 정보 전송
                    {
                        S_EnterGame enterPacket = new S_EnterGame();
                        enterPacket.Player = gameObject.Info;
                        newPlayer.Session.Send(enterPacket);
                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (var player in _players.Values)
                        {
                            if (gameObject != player)
                            {
                                spawnPacket.Objects.Add(player.Info);
                            }
                        }

                        newPlayer.Session.Send(spawnPacket);
                    }
                    break;
                case GameObjectType.Monster:
                    _monsters.Add(gameObject.Info.ObjectId, gameObject as Monster);
                    break;
                case GameObjectType.Projectile:
                    _projectiles.Add(gameObject.Info.ObjectId, gameObject as Projectile);
                    break;
            }

            // 타인들한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (var player in _players.Values)
                {
                    if (gameObject.Id != player.Id)
                    {
                        player.Session.Send(spawnPacket);
                    }
                }
            }
        }
    }

    public void LeaveGame(int objectId)
    {
        var type = ObjectManager.GetObjectTypeById(objectId);
        lock (_lock)
        {
            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player))
                    if (player == null)
                        return;

                player.Room = null;
                Map.ApplyLeave(player);
                {
                    S_LeaveGame leavePk = new S_LeaveGame();
                    player.Session.Send(leavePk);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster))
                    if (monster == null)
                        return;
                monster.Room = null;
                Map.ApplyLeave(monster);
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile))
                    if (projectile == null)
                        return;
                projectile.Room = null;
            }

            {
                S_Despawn despawnPk = new S_Despawn();
                despawnPk.PlayerIds.Add(objectId);
                foreach (var p in _players.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPk);
                }
            }
        }
    }

    public void Update()
    {
        lock (_lock)
        {
            foreach (var item in _projectiles.Values)
            {
                item.Update();
            }
        }
    }

    public void Broadcast(IMessage packet)
    {
        lock (_lock)
        {
            foreach (var player in _players.Values)
            {
                player.Session.Send(packet);
            }
        }
    }

    public void HandleMove(Player player, C_Move movePacket)
    {
        if (player == null)
            return;

        lock (_lock)
        {
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
            {
                if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                {
                    return;
                }
            }

            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;

            Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

            S_Move sMove = new S_Move
            {
                ObjectId = player.Info.ObjectId,
                PosInfo = movePacket.PosInfo
            };
            Broadcast(sMove);
        }
    }

    public void HandleSkill(Player player, C_Skill skillPacket)
    {
        if (player == null)
            return;

        lock (_lock)
        {
            var info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // TODO : 스킬 사용 여부 체크
            info.PosInfo.State = CreatureState.Skill;
            var pk = new S_Skill
            {
                ObjectId = player.Info.ObjectId,
                Info = new SkillInfo()
            };
            pk.Info.SkillId = skillPacket.Info.SkillId;

            Broadcast(pk);


            if (skillPacket.Info.SkillId == 1)
            {
                // 데미지 판정
                var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                var target = Map.Find(skillPos);
                if (target == null)
                    return;

                Console.WriteLine($"Hit Player! {target.Info.ObjectId}");
            }
            else if (skillPacket.Info.SkillId == 2)
            {
                var arrow = ObjectManager.Instance.Add<Arrow>();
                if (arrow == null)
                    return;
                arrow.Owner = player;
                arrow.PosInfo.State = CreatureState.Moving;
                arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                arrow.PosInfo.PosX = player.PosInfo.PosX;
                arrow.PosInfo.PosY = player.PosInfo.PosY;

                EnterGame(arrow);
            }

        }
    }


}
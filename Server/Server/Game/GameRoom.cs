using System;
using System.Collections.Generic;
using Google.Protobuf;
using Protocol;

namespace Server.Game;

public class GameRoom
{
    object _lock = new object();
    public int RoomId { get; set; }

    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    Map _map = new Map();

    public void Init(int mapId)
    {
        _map.LoadMap(mapId);
    }

    public void EnterGame(Player newPlayer)
    {
        if (newPlayer == null)
        {
            return;
        }

        lock (_lock)
        {
            _players.Add(newPlayer.Info.PlayerId, newPlayer);
            newPlayer.Room = this;

            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = newPlayer.Info;
                newPlayer.Session.Send(enterPacket);
                S_Spawn spawnPacket = new S_Spawn();
                foreach (var player in _players.Values)
                {
                    if (newPlayer != player)
                    {
                        spawnPacket.Players.Add(player.Info);
                    }
                }

                newPlayer.Session.Send(spawnPacket);
            }
            // 타인들한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Players.Add(newPlayer.Info);
                foreach (var player in _players.Values)
                {
                    if (newPlayer != player)
                    {
                        player.Session.Send(spawnPacket);
                    }
                }
            }
        }
    }

    public void LeaveGame(int playerId)
    {
        lock (_lock)
        {
            Player player = null;
            if (_players.Remove(playerId, out player))
                if (player == null)
                    return;

            player.Room = null;

            {
                S_LeaveGame leavePk = new S_LeaveGame();
                player.Session.Send(leavePk);
            }

            {
                S_Despawn despawnPk = new S_Despawn();
                despawnPk.PlayerIds.Add(playerId);
                foreach (var p in _players.Values)
                {
                    p.Session.Send(despawnPk);
                }
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
            PlayerInfo info = player.Info;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
            {
                if (_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                {
                    return;
                }
            }

            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;

            _map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

            S_Move sMove = new S_Move
            {
                PlayerId = player.Info.PlayerId,
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

            // 통과
            info.PosInfo.State = CreatureState.Skill;

            var pk = new S_Skill
            {
                PlayerId = player.Info.PlayerId,
                Info = new SkillInfo()
            };
            pk.Info.SkillId = 1;
            Broadcast(pk);

            // 데미지 판정
            var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
            var target = _map.Find(skillPos);
            if (target == null)
                return;

            Console.WriteLine($"Hit Player! {target.Info.PlayerId}");
        }
    }


}
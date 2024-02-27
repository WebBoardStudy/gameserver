using System;
using System.Collections.Generic;
using Google.Protobuf;
using Protocol;

namespace Server.Game;

public class GameRoom
{
    object _lock = new object();
    public int RoomId { get; set; }

    List<Player> _players = new List<Player>();

    public void EnterGame(Player newPlayer)
    {
        if (newPlayer == null)
        {
            return;
        }

        lock (_lock)
        {
            _players.Add(newPlayer);
            newPlayer.Room = this;
            
            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = newPlayer.Info;
                newPlayer.Session.Send(enterPacket);
                S_Spawn spawnPacket = new S_Spawn();
                foreach (var player in _players)
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
                foreach (var player in _players)
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
            Player player = _players.Find(p =>
            {
                return p.Info.PlayerId == playerId;
            });   
            if (player == null)
                return;

            _players.Remove(player);
            player.Room = null;

            {
                S_LeaveGame leavePk = new S_LeaveGame();
                player.Session.Send(leavePk);
            }

            {
                S_Despawn despawnPk = new S_Despawn();
                despawnPk.PlayerIds.Add(playerId);
                foreach (var p in _players)
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
            foreach (var player in _players)
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
            player.Info.PosInfo = movePacket.PosInfo;

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
        }
    }
}
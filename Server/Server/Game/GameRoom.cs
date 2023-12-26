using System.Collections.Generic;
using Google.Protobuf.Protocol;

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
}
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        private object _lock = new object();
        public int RoomId { get; set; }

        private List<Player> _players = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;
                
                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = new 
                }
                
                // 타인한테 정보 전송
            }
        }

        public void LeaveGame(int playerId)
        {
        }
    }
}
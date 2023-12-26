using System.Collections.Generic;

namespace Server.Game;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new PlayerManager();
    object _lock = new object();
    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private int _playerIndex = 1;

    public Player Add()
    {
        Player player = new Player();
        lock (_lock)
        {
            player.Info.PlayerId = _playerIndex;
            _players.Add(_playerIndex, player);
            _playerIndex++;
        }

        return player;
    }

    public bool Remove(int playerId)
    {
        lock (_lock)
        {
            return _players.Remove(playerId);
        }
    }

    public Player Find(int playerId)
    {
        lock (_lock)
        {
            Player player;
            if (_players.TryGetValue(playerId, out player))
                return player;
            
            return null;
        }
    }
}
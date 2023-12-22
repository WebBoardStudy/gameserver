using System.Collections.Generic;

namespace Server.Game;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new PlayerManager();
    object _lock = new object();
    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    int _plaeyrId = 1; //TODO

    public Player Add()
    {
        Player player = new Player();
        lock (_lock)
        {
            player.info.PlayerId = _plaeyrId;
            _players.Add(_plaeyrId, player);
            _plaeyrId++;
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
            Player player = null;
            if (_players.TryGetValue(playerId, out player))
                return player;
            return null;
        }
    }
}
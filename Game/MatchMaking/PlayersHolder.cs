using System.Collections;

namespace ITask6.Game.MatchMaking;

public class PlayersHolder
{
    private readonly Dictionary<string,string> _players = new();

    public bool CanAddWith(string id, string nickname)
    {
        return !_players.ContainsKey(id) && !_players.ContainsValue(nickname);
    }

    public void AddNew(string id, string nickname)
    {
        _players[id] = nickname;
    }

    public void Remove(string id)
    {
        _players.Remove(id);
    }
    
    public string GetPlayerName(string id)
    {
        return _players.GetValueOrDefault(id, "noname");
    }

    public IEnumerable<string> GetFree(RoomsHolder roomsHolder)
    {
        return _players.Keys.Where(p => !roomsHolder.HasPlayerInRoom(p));
    }
}
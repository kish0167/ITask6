using ITask6.Game.TicTacToe;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.Services;

public class MatchMakingService : IMatchMakingService
{
    private readonly Dictionary<string,string> _players = new();
    private readonly List<Room> _rooms = new();
    private const bool DestroyEmptyRooms = true;
    
    public bool TryToAddPlayer(string id, string nickname)
    {
        if (!_players.ContainsKey(id) && !_players.ContainsValue(nickname))
        {
            _players[id] = nickname;
            return true;
        }
        return false;
    }

    public void RemovePlayer(string id)
    {
        _players.Remove(id);
        RemoveUserFromRoom(id);
    }

    public List<Room> GetRooms()
    {
        return _rooms;
    }

    public Room? JoinRoom(string id, int roomId)
    {
        if (!PlayerHasNickname(id) || PlayerIsInRoom(id)) return null;
        foreach (Room room in _rooms)
        {
            if (room.Id != roomId || !room.IsAvailable) continue;
            room.AddPlayer(id, _players[id]);
            return room;
        }
        return null;
    }

    public void LeaveRoom(string id)
    {
        RemoveUserFromRoom(id);
    }

    public Room? CreateRoomAndJoin(string id, Hub hub)
    {
        if (!PlayerIsAbleToJoinRoom(id)) return null;
        return JoinRoom(id, CreateRoom(hub));
    }

    private bool PlayerHasNickname(string id)
    {
        return _players.ContainsKey(id);
    }
    
    private int CreateRoom(Hub hub)
    {
        Room room = new Room(2, hub);
        //Room room = new TicTacToeRoom(hub);
        _rooms.Add(room);
        return room.Id;
    }
    
    private bool PlayerIsInRoom(string id)
    {
        foreach (Room room in _rooms)
        {
            if (room.ContainsPlayer(id)) return true;
        }
        return false;
    }

    private void RemoveUserFromRoom(string id)
    {
        foreach (Room room in _rooms)
        {
            if (room.ContainsPlayer(id))
            {
                room.RemovePlayer(id);
            }
        }
        if (DestroyEmptyRooms) DeleteEmptyRooms();
    }

    private void DeleteEmptyRooms()
    {
        _rooms.RemoveAll(r => r.IsEmpty());
    }
    
    private bool PlayerIsAbleToJoinRoom(string id)
    {
        return PlayerHasNickname(id) && !PlayerIsInRoom(id);
    }
}
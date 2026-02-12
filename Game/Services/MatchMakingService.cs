using ITask6.Game.Connection;
using ITask6.Game.TicTacToe;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.Services;

public class MatchMakingService : IMatchMakingService
{
    private readonly Dictionary<string,string> _players = new();
    private readonly List<Room> _rooms = new();
    private const bool DestroyEmptyRooms = true;
    private const int DefaultRoomCapacity = 4;
    
    public bool TryToAddPlayer(string id, string nickname)
    {
        if (_players.ContainsKey(id) || _players.ContainsValue(nickname)) return false;
        _players[id] = nickname;
        return true;
    }

    public async Task RemovePlayer(string id)
    {
        _players.Remove(id);
        await RemovePlayerFromRoom(id);
    }

    public List<Room> GetRooms()
    {
        return _rooms;
    }

    public async Task<Room?> JoinRoom(string id, int roomId)
    {
        if (!PlayerIsAbleToJoinRoom(id)) return null;
        foreach (Room room in _rooms)
        {
            if (room.Id != roomId || !room.IsAvailable) continue;
            await room.AddPlayer(id, _players[id]);
            return room;
        }
        return null;
    }
    
    public async Task PlayerAction(string id, string type, string data)
    {
        foreach (Room room in _rooms)
        {
            if (!room.ContainsPlayer(id)) continue;
            await room.PlayerAction(id, type, data);
            return;
        }
    }

    public async Task<Room?> CreateTicTacToeRoomAndJoin(string id, string roomName, int dimension, IHubContext<GameHub> hubContext)
    {
        if (!PlayerIsAbleToJoinRoom(id)) return null;
        return await JoinRoom(id, CreateTicTacToeRoom(roomName, dimension, hubContext));
    }

    private bool PlayerHasNickname(string id)
    {
        return _players.ContainsKey(id);
    }
    
    private int CreateTicTacToeRoom(string name, int dimension, IHubContext<GameHub> hubContext)
    {
        Room room = new TicTacToeRoom(name, dimension, hubContext);
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

    public async Task RemovePlayerFromRoom(string id)
    {
        foreach (Room room in _rooms)
        {
            if (room.ContainsPlayer(id))
            {
                await room.RemovePlayer(id);
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
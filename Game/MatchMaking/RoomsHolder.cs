namespace ITask6.Game.MatchMaking;

public class RoomsHolder
{
    private readonly List<Room> _rooms = new();
    public List<Room> Rooms  => _rooms;

    public event Action? OnAnyChange; 

    public bool HasRoomWithName(string roomName)
    {
        return _rooms.Any(r => r.Name == roomName);
    }

    public async Task RemovePlayerFromRoom(string id)
    {
        foreach (Room room in _rooms)
        {
            if (room.ContainsPlayer(id)) await room.RemovePlayer(id);
            OnAnyChange?.Invoke();
            return;
        }
    }

    public async Task<Room?> AddPlayerToRoom(string id, string name, int roomId)
    {
        foreach (Room room in _rooms)
        {
            if (room.Id == roomId)
            {
                await room.AddPlayer(id, name);
                OnAnyChange?.Invoke();
                return room;
            }
        }

        return null;
    }

    public async Task PlayerAction(string playerId, string type, string data)
    {
        foreach (Room room in _rooms)
        {
            if (!room.ContainsPlayer(playerId)) continue;
            await room.PlayerAction(playerId, type, data);
            return;
        }
    }

    public void DeleteEmptyRooms()
    {
        _rooms.RemoveAll(r => r.IsEmpty());
        OnAnyChange?.Invoke();
    }

    public bool RoomExists(string name)
    {
        return _rooms.Any(r => r.Name == name);
    }

    public void Add(Room room)
    {
        _rooms.Add(room);
        OnAnyChange?.Invoke();
    }

    public bool HasPlayerInRoom(string id)
    {
        return _rooms.Any(r => r.ContainsPlayer(id));
    }
}
namespace ITask6.Game.MatchMaking;

public class RoomsHolder
{
    private readonly List<Room> _rooms = new();
    public List<Room> Rooms  => _rooms;

    public bool HasRoomWithName(string roomName)
    {
        return _rooms.Select(r => r.Name == roomName).Any();
    }

    public async Task RemovePlayerFromRoom(string id)
    {
        foreach (Room room in _rooms)
        {
            if (room.ContainsPlayer(id)) await room.RemovePlayer(id);
        }
    }

    public async Task<Room?> AddPlayerToRoom(string id, string name, int roomId)
    {
        foreach (Room room in _rooms)
        {
            if (room.Id == roomId)
            {
                await room.AddPlayer(id, name);
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
    }

    public bool RoomExists(string name)
    {
        return _rooms.Any(r => r.Name == name);
    }

    public void Add(Room room)
    {
        _rooms.Add(room);
    }

    public bool PlayerIsInRoom(string id)
    {
        return _rooms.Any(r => r.ContainsPlayer(id));
    }
}
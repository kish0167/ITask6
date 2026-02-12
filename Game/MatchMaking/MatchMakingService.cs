using ITask6.Game.Connection;
using ITask6.Game.TicTacToe;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.MatchMaking;

public class MatchMakingService(IHubContext<GameHub> hubContext) : IHostedService
{
    private readonly PlayersHolder _playersHolder = new();
    private readonly RoomsHolder _roomsHolder = new();
    private readonly IHubContext<GameHub> _hubContext = hubContext;

    private const bool DestroyEmptyRooms = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _roomsHolder.OnAnyChange += OnAnyRoomChangeCallback;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _roomsHolder.OnAnyChange -= OnAnyRoomChangeCallback;
        return Task.CompletedTask;
    }
    
    private void OnAnyRoomChangeCallback()
    {
        _ = SendRoomsToFreePlayers();
    }

    private async Task SendRoomsToFreePlayers()
    {
        try
        {
            foreach (string id in _playersHolder.GetFree(_roomsHolder))
            {
                await _hubContext.Clients.Client(id).SendAsync("updateRooms", _roomsHolder.Rooms);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public bool TryToAddPlayer(string id, string nickname)
    {
        if (!_playersHolder.CanAddWith(id, nickname)) return false;
        _playersHolder.AddNew(id, nickname);
        return true;
    }

    public bool HasRoomWithName(string roomName)
    {
        return _roomsHolder.HasRoomWithName(roomName);
    }

    public async Task RemovePlayer(string id)
    {
        _playersHolder.Remove(id);
        await RemovePlayerFromRoom(id);
    }

    public List<Room> GetRooms()
    {
        return _roomsHolder.Rooms;
    }

    public async Task<Room?> JoinRoom(string id, int roomId)
    {
        if (!PlayerIsAbleToJoinRoom(id)) return null;
        return await _roomsHolder.AddPlayerToRoom(id, _playersHolder.GetPlayerName(id), roomId);
    }
    
    public async Task PlayerAction(string id, string type, string data)
    {
        await _roomsHolder.PlayerAction(id, type, data);
    }

    public async Task<Room?> CreateTicTacToeRoomAndJoin(string id, string roomName, int dimension, IHubContext<GameHub> hubContext)
    {
        if (!PlayerIsAbleToJoinRoom(id)) return null;
        Room? room = CreateTicTacToeRoom(roomName, dimension, hubContext);
        if (room == null) return room;
        return await JoinRoom(id, room.Id);
    }
    
    private Room? CreateTicTacToeRoom(string name, int dimension, IHubContext<GameHub> hubContext)
    {
        if (_roomsHolder.RoomExists(name)) return null;
        Room room = new TicTacToeRoom(name, dimension, hubContext);
        _roomsHolder.Add(room);
        return room;
    }
    
    private bool PlayerIsInRoom(string id)
    {
        return _roomsHolder.HasPlayerInRoom(id);
    }

    public async Task RemovePlayerFromRoom(string id)
    {
        await _roomsHolder.RemovePlayerFromRoom(id);
        if (DestroyEmptyRooms) _roomsHolder.DeleteEmptyRooms();
    }
    
    private bool PlayerIsAbleToJoinRoom(string id)
    {
        return !PlayerIsInRoom(id);
    }
}
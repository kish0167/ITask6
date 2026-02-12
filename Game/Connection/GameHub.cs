using ITask6.Game.MatchMaking;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.Connection;

public class GameHub : Hub
{
    private readonly MatchMakingService _matchMakingService;
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(MatchMakingService matchMakingService, IHubContext<GameHub> hubContext)
    {
        _matchMakingService = matchMakingService;
        _hubContext = hubContext;
    }

    public async Task<bool> TryToConnect(string nickname)
    {
        await Task.CompletedTask;
        bool isSuccessful = _matchMakingService.TryToAddPlayer(Context.ConnectionId, nickname);
        return isSuccessful;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _matchMakingService.RemovePlayer(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<List<Room>> GetRooms()
    {
        return _matchMakingService.GetRooms();
    }

    public async Task<Room?> CreateOwnRoom(string roomName, int dimension)
    {
        if (_matchMakingService.HasRoomWithName(roomName))
        {
            await Clients.Client(Context.ConnectionId).SendAsync("systemMessage", 
                "Room with this name already exists");
            return null;
        }
        return await _matchMakingService.CreateTicTacToeRoomAndJoin(Context.ConnectionId, roomName, dimension, _hubContext);
    }

    public async Task<Room?> JoinRoom(int roomId)
    {
        return await _matchMakingService.JoinRoom(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom()
    {
        await _matchMakingService.RemovePlayerFromRoom(Context.ConnectionId);
    }

    public async Task GameAction(string type, string data)
    {
        await _matchMakingService.PlayerAction(Context.ConnectionId, type, data);
    }
}
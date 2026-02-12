using ITask6.Game.Services;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.Connection;

public class GameHub : Hub
{
    private IMatchMakingService _matchMakingService;
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(IMatchMakingService matchMakingService, IHubContext<GameHub> hubContext)
    {
        _matchMakingService = matchMakingService;
        _hubContext = hubContext;
    }

    public async Task<bool> TryToConnect(string nickname)
    {
        bool isSuccessful = _matchMakingService.TryToAddPlayer(Context.ConnectionId, nickname);
        await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("message", "ur gay");
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

    public async Task<Room?> CreateOwnRoom()
    {
        return await _matchMakingService.CreateRoomAndJoin(Context.ConnectionId, _hubContext);
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

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("message", Context.ConnectionId, message);
    }
}
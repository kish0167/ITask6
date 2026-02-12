using ITask6.Game.Connection;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.TicTacToe;

public class TicTacToeRoom(IHubContext<GameHub> hubContext) : Room(2, hubContext)
{
    private readonly TicTacToeGameBoard _board = new(3);
    private readonly TicTacToeStateManager _stateManager = new();
    private readonly TicTacToePlayerManager _playerManager = new();
    private readonly TicTacToeMoveValidator _moveValidator = new();
    protected override async Task OnPlayerAction(string id, string type, string action)
    {
        await base.OnPlayerAction(id, type, action);
        switch (type)
        {
            case "ticTacToeStart":
                await HandleStartGame(id);
                break;
            case "ticTacToeMove":
                await HandleMakeMove(id, action);
                break;
        }
    }

    protected override async Task OnPlayerAdded(string id)
    {
        await base.OnPlayerAdded(id);
        if (!CanStart())
        {
            await BroadcastState();
            return;
        }
        _playerManager.AssignSides(Players.ElementAt(0), Players.ElementAt(1));
        _stateManager.StartGame();
        await BroadcastState();
    }

    protected override async Task OnPlayerRemoved(string id)
    {
        await base.OnPlayerRemoved(id);
        _stateManager.EndGame();
        if (Players.Count() != 0)
        {
            await BroadcastState();
            await SendSystemMessage(Players.ElementAt(0), "Your opponent left the room");
        }
        _board.Reset();
    }

    private async Task HandleStartGame(string id)
    {
        if (!CanStart())
        {
            await SendSystemMessage(id, "Can't start game now");
            return;
        }
        _playerManager.AssignSides(Players.ElementAt(0), Players.ElementAt(1));
        _stateManager.StartGame();
        await BroadcastState();
    }

    private async Task HandleMakeMove(string id, string action)
    {
        if (!_moveValidator.IsValidMove(_board, id, action, _stateManager, _playerManager, out int row, out int col))
            return;
        _board.PlaceMove(row, col, _stateManager.GetCurrentPlayerValue());
        await HandleNextGameState();
    }

    private async Task HandleNextGameState()
    {
        if (_board.WinCondition())
        {
            await EndGame(false);
        }
        else if (_board.IsFull())
        {
            await EndGame(true);
        }
        else
        {
            _stateManager.SwitchTurn();
            await BroadcastState();
        }
    }

    private async Task EndGame(bool draw)
    {
        await BroadcastEndState(draw);
        _stateManager.EndGame();
        _board.Reset();
    }
    
    private bool CanStart()
    {
        return PlayerNames.Count == 2 && _stateManager.IsWaiting;
    }
    
    private async Task BroadcastState()
    {
        foreach (string id in Players)
        {
            TicTacToeGameStateDto state = BuildDtoForPlayer(id);
            await SendDataToPlayer(id, "gameState", state);
        }
    }
    
    private async Task SendSystemMessage(string id, string message)
    {
        await SendDataToPlayer(id, "systemMessage", message);
    }
    
    private async Task BroadcastEndState(bool draw)
    {
        string? winnerId = draw ? null : _playerManager.GetPlayerId(_stateManager.CurrentStage);
        string? winnerName = draw ? null : PlayerNames[_playerManager.GetPlayerId(_stateManager.CurrentStage)];
        
        foreach (string id in Players)
        {
            TicTacToeGameStateDto state = BuildDtoForPlayer(id);
            state.WinnerId = winnerId;
            state.WinnerName = winnerName;
            state.IsDraw = draw;
            state.Phase = "finished";
            await SendDataToPlayer(id, "gameState", state);
        }
    }
    
    private TicTacToeGameStateDto BuildDtoForPlayer(string id)
    {
        string? opponentId = Players.FirstOrDefault(pid => pid != id);
        TicTacToeGameStateDto state = new()
        {
            Board = _board.Grid,
            Phase = _stateManager.CurrentStage.ToString(),
            CurrentTurnName = _stateManager.CurrentStage is TicTacToeGameStage.XTurn or TicTacToeGameStage.OTurn 
                ?  PlayerNames[_playerManager.GetPlayerId(_stateManager.CurrentStage)]
                : null,
            YourSide = _playerManager.GetPlayerSide(id),
            OpponentId = opponentId,
            OpponentName = opponentId != null ? PlayerNames[opponentId] : null,
            IsYourTurn = _playerManager.IsCurrentPlayerTurn(id, _stateManager.CurrentStage)
        };
        return state;
    }
}
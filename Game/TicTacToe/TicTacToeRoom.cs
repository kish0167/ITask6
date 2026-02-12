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
    
    private async Task HandleStartGame(string id)
    {
        if (!CanStart()) return;
        _playerManager.AssignSides(Players.Keys.ElementAt(0), Players.Keys.ElementAt(1));
        _stateManager.StartGame();
        await BroadcastState();
    }

    private async Task HandleMakeMove(string id, string action)
    {
        if (!_moveValidator.IsValidMove(_board, id, action, _stateManager, _playerManager, out int row, out int col))
            return;
        _board.PlaceMove(row, col, _stateManager.GetCurrentPlayerValue());
        
        await HandleAfterMove();
    }

    private async Task HandleAfterMove()
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

    private async Task BroadcastState()
    {
        foreach (string id in Players.Keys)
        {
            TicTacToeGameStateDto state = BuildDtoForPlayer(id);
            await SendDataToPlayer(id, "gameState", state);
            await SendDataToPlayer(id, "message", "fuck you");
        }
    }
    
    private async Task BroadcastEndState(bool draw)
    {
        string? winnerId = draw ? null : _playerManager.GetPlayerId(_stateManager.CurrentState);
        string? winnerName = draw ? null : Players[_playerManager.GetWinnerId(_stateManager.CurrentState)];
        
        foreach (string id in Players.Keys)
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
        string? opponentId = Players.Keys.FirstOrDefault(pid => pid != id);
        TicTacToeGameStateDto state = new()
        {
            Board = _board.Grid,
            Phase = _stateManager.CurrentState.ToString(),
            CurrentTurnName = _stateManager.CurrentState is TicTacToeGameState.XTurn or TicTacToeGameState.OTurn 
                ?  Players[_playerManager.GetPlayerId(_stateManager.CurrentState)]
                : null,
            YourSide = _playerManager.GetPlayerSide(id),
            OpponentId = opponentId,
            OpponentName = opponentId != null ? Players[opponentId] : null,
            IsYourTurn = _playerManager.IsCurrentPlayerTurn(id, _stateManager.CurrentState)
        };
        return state;
    }

    private async Task EndGame(bool draw)
    {
        await BroadcastEndState(draw);
        _stateManager.EndGame();
        _board.Reset();
    }

    private bool CanStart()
    {
        return Players.Count == 2 && _stateManager.IsWaiting;
    }
}
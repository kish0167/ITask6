using ITask6.Game.Connection;
using ITask6.Game.Score;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.TicTacToe;

public class TicTacToeRoom : Room
{
    private readonly TicTacToeGameBoard _board;
    private readonly TicTacToeStateManager _stateManager = new();
    private readonly TicTacToePlayerManager _playerManager = new();
    private readonly TicTacToeMoveValidator _moveValidator = new();

    public TicTacToeRoom(string name, int dimension, IHubContext<GameHub> hubContext) : base(2, name, hubContext)
    {
        _board = new(dimension);
        RoomInfo = $"board: {dimension}x{dimension}";
        if (dimension > 4) RoomInfo += " - 4 in a row to win";
    }

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
        await BroadcastState();
    }

    protected override async Task OnPlayerRemoved(string id)
    {
        await base.OnPlayerRemoved(id);
        _stateManager.Reset();
        if (Players.Count() != 0 && _stateManager.CurrentStage != TicTacToeGameStage.Ended)
        {
            await BroadcastState();
            await SendSystemMessage(Players.ElementAt(0), "Your opponent has left the room");
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
        string winnerId = _playerManager.GetPlayerId(_stateManager.CurrentStage);
        if(!draw) await AddScore(winnerId);
        _stateManager.EndGame();
        await BroadcastEndState(draw, winnerId);
        _board.Reset();
    }

    private async Task AddScore(string winnerId)
    {
        await ScoreManager.AddScoreAsync(PlayerNames[winnerId], _board.Dimension * _board.Dimension);
    }

    private bool CanStart()
    {
        return PlayerNames.Count == 2 && (_stateManager.IsWaitingOrEnded());
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
    
    private async Task BroadcastEndState(bool draw, string winnerId)
    {
        string? winnerName = draw ? null : PlayerNames[winnerId];
        
        foreach (string id in Players)
        {
            TicTacToeGameStateDto state = BuildDtoForPlayer(id);
            state.WinnerId = draw ? null : winnerId;
            state.WinnerName = winnerName;
            state.IsDraw = draw;
            await SendDataToPlayer(id, "gameState", state);
        }
    }
    
    private TicTacToeGameStateDto BuildDtoForPlayer(string id)
    {
        string? opponentId = Players.FirstOrDefault(pid => pid != id);
        TicTacToeGameStateDto state = new()
        {
            Board = _board.Grid,
            Dimension = _board.Dimension,
            Phase = _stateManager.CurrentStage.ToString(),
            CurrentTurnName = _stateManager.CurrentStage is TicTacToeGameStage.XTurn or TicTacToeGameStage.OTurn 
                ?  PlayerNames[_playerManager.GetPlayerId(_stateManager.CurrentStage)]
                : null,
            YourName = PlayerNames[id],
            YourSide = _playerManager.GetPlayerSide(id),
            OpponentId = opponentId,
            OpponentName = opponentId != null ? PlayerNames[opponentId] : null,
            IsYourTurn = _playerManager.IsCurrentPlayerTurn(id, _stateManager.CurrentStage)
        };
        return state;
    }
}
using System.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.TicTacToe;

public class TicTacToeRoom(Hub hub) : Room(2, hub)
{
    private readonly Dictionary<TicTacToeGameState, string> _playerSides = new();
    
    private TicTacToeGameState _state = TicTacToeGameState.Waiting;
    private readonly List<List<int>> _gameField =
    [
        [0, 0, 0],
        [0, 0, 0],
        [0, 0, 0]
    ];
    
    private readonly IReadOnlyDictionary<TicTacToeGameState, int> _moveRepresentation =
        new Dictionary<TicTacToeGameState, int>
        {
            { TicTacToeGameState.XTurn , 1},
            { TicTacToeGameState.OTurn , 2}
        };

    private const int Dimension = 3;
    
    protected override void OnGameStarted()
    {
        DecidePlayerSides();
    }

    public override bool CanStartGame()
    {
        return Players.Count == 2;
    }

    public override void PlayerAction(string id, string type, string action)
    {
        if (!IsValidAction(type)) return;
        if (!IsValidMove(id, action, out int move)) return;
        MakeMove(move);
        UpdateGameState();
    }

    private void DecidePlayerSides()
    {
        string[] players = Players.Keys.ToArray();
        if (RandomNumberGenerator.GetInt32(2) == 0)
        {
            _playerSides[TicTacToeGameState.XTurn] = players[0];
            _playerSides[TicTacToeGameState.OTurn] = players[1];
        }
        else
        {
            _playerSides[TicTacToeGameState.XTurn] = players[1];
            _playerSides[TicTacToeGameState.OTurn] = players[0];
        }
    }
    
    private void MakeMove(int move)
    {
        _gameField[move / Dimension][move % Dimension] = _moveRepresentation[_state];
    }
    
    private void UpdateGameState()
    {
        if (WinCondition()) return;
    }

    private bool IsValidAction(string type)
    {
        return string.Equals(type, "ticTacToeMove");
    }
    
    private bool IsValidMove(string id, string action, out int outMove)
    {
        if (!int.TryParse(action, out int move) || move < 0 || move >= Dimension * Dimension)
        {
            outMove = -1;
            return false;
        }
        outMove = move;
        return _gameField[move/3][move%3] == 0 && IsUserMove(id);
    }

    private bool IsUserMove(string id)
    {
        return _playerSides.ContainsKey(_state) && _playerSides[_state] == id;
    }
    
    private bool WinCondition()
    {
        for (int i = 0; i < Dimension; i++)
        {
            
        }

        return false;
    }
    
}
namespace ITask6.Game.TicTacToe;

public class TicTacToeStateManager
{
    public TicTacToeGameState CurrentState { get; private set; } = TicTacToeGameState.Waiting;
    private readonly Dictionary<TicTacToeGameState, int> _moveRepresentation = new()
    {
        { TicTacToeGameState.XTurn, 1 },
        { TicTacToeGameState.OTurn, 2 }
    };
    
    public bool IsWaiting => CurrentState == TicTacToeGameState.Waiting;
    
    public int GetCurrentPlayerValue() => _moveRepresentation[CurrentState];
    
    public void StartGame() => CurrentState = TicTacToeGameState.XTurn;
    
    public void SwitchTurn() => 
        CurrentState = CurrentState == TicTacToeGameState.XTurn 
            ? TicTacToeGameState.OTurn 
            : TicTacToeGameState.XTurn;
    
    public void EndGame() => CurrentState = TicTacToeGameState.Waiting;
}
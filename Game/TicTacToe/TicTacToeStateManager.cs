namespace ITask6.Game.TicTacToe;

public class TicTacToeStateManager
{
    public TicTacToeGameStage CurrentStage { get; private set; } = TicTacToeGameStage.Waiting;
    private readonly Dictionary<TicTacToeGameStage, int> _moveRepresentation = new()
    {
        { TicTacToeGameStage.XTurn, 1 },
        { TicTacToeGameStage.OTurn, 2 }
    };
    
    public bool IsWaiting => CurrentStage == TicTacToeGameStage.Waiting;
    
    public int GetCurrentPlayerValue() => _moveRepresentation[CurrentStage];
    
    public void StartGame() => CurrentStage = TicTacToeGameStage.XTurn;
    
    public void SwitchTurn() => 
        CurrentStage = CurrentStage == TicTacToeGameStage.XTurn 
            ? TicTacToeGameStage.OTurn 
            : TicTacToeGameStage.XTurn;
    
    public void EndGame() => CurrentStage = TicTacToeGameStage.Ended;
}
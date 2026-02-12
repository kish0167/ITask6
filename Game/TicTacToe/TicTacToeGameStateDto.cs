using Newtonsoft.Json;

namespace ITask6.Game.TicTacToe;

public class TicTacToeGameStateDto
{
    [JsonProperty("board")]
    public int[][] Board { get; set; } = [];
    [JsonProperty("dimension")]
    public int Dimension { get; set; }
    [JsonProperty("phase")]
    public string Phase { get; set; } = "waiting";
    [JsonProperty("currentTurnName")]
    public string? CurrentTurnName { get; set; }
    [JsonProperty("yourName")]
    public string? YourName { get; set; }
    [JsonProperty("yourSide")]
    public string? YourSide { get; set; }
    [JsonProperty("opponentId")]
    public string? OpponentId { get; set; }
    [JsonProperty("opponentName")]
    public string? OpponentName { get; set; }
    [JsonProperty("winnerId")]
    public string? WinnerId { get; set; }
    [JsonProperty("winnerName")]
    public string? WinnerName { get; set; }
    [JsonProperty("isDraw")]
    public bool IsDraw { get; set; }
    [JsonProperty("isYourTurn")]
    public bool IsYourTurn { get; set; }
}
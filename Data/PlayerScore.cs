using System.ComponentModel.DataAnnotations;

namespace ITask6.Data;

public class PlayerScore
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Nickname { get; set; } = string.Empty;
    public int Score { get; set; }
}
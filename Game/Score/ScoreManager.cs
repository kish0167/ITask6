using ITask6.Data;
using Microsoft.EntityFrameworkCore;

namespace ITask6.Game.Score;

public static class ScoreManager
{
    private static IServiceProvider? _serviceProvider;
    
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static async Task AddScoreAsync(string nickname, int value)
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("ScoreManager not initialized");
        }
        
        using IServiceScope scope = _serviceProvider.CreateScope();
        LeaderboardContext db = scope.ServiceProvider.GetRequiredService<LeaderboardContext>();

        PlayerScore? existing = await db.PlayerScores
            .FirstOrDefaultAsync(p => p.Nickname == nickname);

        if (existing == null)
        {
            db.PlayerScores.Add(new PlayerScore 
            { 
                Nickname = nickname, 
                Score = value 
            });
        }
        else if (value > existing.Score)
        {
            existing.Score += value;
        }

        await db.SaveChangesAsync();
    }
}
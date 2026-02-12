using Microsoft.EntityFrameworkCore;

namespace ITask6.Data;

public class LeaderboardContext : DbContext
{
    public LeaderboardContext(DbContextOptions<LeaderboardContext> options) 
        : base(options) { }

    public DbSet<PlayerScore> PlayerScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerScore>()
            .HasIndex(p => p.Score)
            .IsDescending();
        
        modelBuilder.Entity<PlayerScore>()
            .HasIndex(p => p.Nickname)
            .IsUnique();
    }
}
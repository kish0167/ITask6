using ITask6.Data;
using ITask6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITask6.Controllers;

public class LeaderboardController(LeaderboardContext db) : Controller
{
    public async Task<IActionResult> Index(int top = 100)
    {
        var scores = await db.PlayerScores
            .OrderByDescending(p => p.Score)
            .Take(top)
            .Select(p => new LeaderboardViewModel
            {
                Rank = 0,
                Nickname = p.Nickname,
                Score = p.Score,
            })
            .ToListAsync();

        for (int i = 0; i < scores.Count; i++)
        {
            scores[i].Rank = i + 1;
        }

        return View(scores);
    }
}
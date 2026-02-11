using System.Diagnostics;
using ITask6.Game.Connection;
using Microsoft.AspNetCore.Mvc;
using ITask6.Models;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Controllers;

public class HomeController : Controller
{
    private IHubContext<GameHub> _hubContext;

    public HomeController(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SendMessage(string message)
    {
        await _hubContext.Clients.All.SendAsync("message", message);
        return Ok();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
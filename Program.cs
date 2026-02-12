using ITask6.Data;
using ITask6.Game.Connection;
using ITask6.Game.MatchMaking;
using ITask6.Game.Score;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<MatchMakingService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<MatchMakingService>());
builder.Services.AddDbContext<LeaderboardContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeaderboardContext>();
    db.Database.Migrate();
}*/

ScoreManager.Initialize(app.Services);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<GameHub>("/Gamehub");

app.Run();
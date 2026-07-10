using DarkPortal.Data;
using DarkPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace DarkPortal.Services;

public class AchievementService
{
    private readonly AppDbContext _db;
    private readonly LessonService _lessonService;

    public AchievementService(AppDbContext db, LessonService lessonService)
    {
        _db = db;
        _lessonService = lessonService;
    }

    public async Task<List<Models.Achievement>> GetAchievements(string userId)
    {
        var achievements = new List<Models.Achievement>();
        if (string.IsNullOrEmpty(userId)) return achievements;

        var completedLessons = await _lessonService.GetCompletedCount(userId);
        var user = await _db.Users.FindAsync(userId);
        var daysSinceRegistration = user != null ? (DateTime.UtcNow - user.CreatedAt).TotalDays : 0;

        var forumMessages = await _db.ForumReplies.CountAsync(r => r.AuthorId == userId);
        var wallMessages = await _db.ProfileComments.CountAsync(c => c.AuthorId == userId);
        var totalMessages = forumMessages + wallMessages;

        achievements.Add(new Models.Achievement
        {
            Id = "student",
            Name = "🎓 Ученик",
            Description = "Пройти 3 урока",
            Icon = Icons.Material.Filled.School,
            Color = "Success",
            Unlocked = completedLessons >= 3
        });
        achievements.Add(new Models.Achievement
        {
            Id = "defender",
            Name = "🛡️ Защитник",
            Description = "Пройти 7 уроков",
            Icon = Icons.Material.Filled.Shield,
            Color = "Primary",
            Unlocked = completedLessons >= 7
        });
        achievements.Add(new Models.Achievement
        {
            Id = "ghost",
            Name = "👻 Призрак",
            Description = "Пройти все 10 уроков",
            Icon = Icons.Material.Filled.VisibilityOff,
            Color = "Tertiary",  // было "Secondary"
            Unlocked = completedLessons >= 10
        });
        achievements.Add(new Models.Achievement
        {
            Id = "talker",
            Name = "💬 Общительный",
            Description = "10+ сообщений",
            Icon = Icons.Material.Filled.Chat,
            Color = "Tertiary",
            Unlocked = totalMessages >= 10
        });
        achievements.Add(new Models.Achievement
        {
            Id = "flash",
            Name = "⚡ Молниеносный",
            Description = "7+ дней на сайте",
            Icon = Icons.Material.Filled.FlashOn,
            Color = "Warning",
            Unlocked = daysSinceRegistration >= 7
        });

        return achievements;
    }
}
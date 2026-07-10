using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class LessonService
{
    private readonly AppDbContext _db;

    public LessonService(AppDbContext db) => _db = db;

    public async Task<int> GetCompletedCount(string userId) =>
        await _db.LessonProgresses.CountAsync(lp => lp.UserId == userId && lp.Completed);

    public async Task<bool> IsLessonCompleted(string userId, string lessonId) =>
        await _db.LessonProgresses.AnyAsync(lp => lp.UserId == userId && lp.LessonId == lessonId && lp.Completed);

    public async Task MarkCompleted(string userId, string lessonId)
    {
        if (!await IsLessonCompleted(userId, lessonId))
        {
            _db.LessonProgresses.Add(new LessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                Completed = true,
                CompletedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> CanAccessLesson(string userId, string lessonId)
    {
        if (string.IsNullOrEmpty(userId)) return false;

        var lessonOrder = GetLessonOrder(lessonId);
        if (lessonOrder <= 1) return true;

        var previousLessonId = GetLessonIdByOrder(lessonOrder - 1);
        return await _db.LessonProgresses.AnyAsync(lp =>
            lp.UserId == userId && lp.LessonId == previousLessonId && lp.Completed);
    }

    public async Task<List<LessonProgress>> GetUserProgress(string userId) =>
        await _db.LessonProgresses.Where(lp => lp.UserId == userId).ToListAsync();

    private int GetLessonOrder(string lessonId) => lessonId switch
    {
        "data-is-oil" => 1,
        "browser-wars" => 2,
        "vpn-mask" => 3,
        "onion-layer" => 4,
        "darknet" => 5,
        "pgp" => 6,
        "simswap" => 7,
        "phishing" => 8,
        "crypto-lesson" => 9,
        "opsec" => 10,
        _ => 99
    };

    private string GetLessonIdByOrder(int order) => order switch
    {
        1 => "data-is-oil",
        2 => "browser-wars",
        3 => "vpn-mask",
        4 => "onion-layer",
        5 => "darknet",
        6 => "pgp",
        7 => "simswap",
        8 => "phishing",
        9 => "crypto-lesson",
        10 => "opsec",
        _ => ""
    };
}
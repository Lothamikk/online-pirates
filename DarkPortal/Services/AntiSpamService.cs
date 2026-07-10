using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class AntiSpamService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public AntiSpamService(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ========== ДЛЯ РЕГИСТРАЦИИ ==========
    public bool CanRegister(string ip)
    {
        return true;
    }

    // ========== ДЛЯ ФОРУМА И КОММЕНТАРИЕВ (С КОНТЕНТОМ) ==========
    public async Task<SpamCheckResult> CheckSpamAsync(string userId, string content)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new SpamCheckResult { IsBlocked = true, Message = "Пользователь не найден" };

        // Проверка на бан
        if (await _userManager.IsLockedOutAsync(user))
            return new SpamCheckResult { IsBlocked = true, Message = "Вы заблокированы" };

        // Проверка на спам-слова
        var spamWords = new[] { "спам", "реклама", "xxx", "порно", "casino", "заработок", "казино", "бесплатно", "заработай" };
        var lowerContent = content.ToLower();
        foreach (var word in spamWords)
        {
            if (lowerContent.Contains(word))
                return new SpamCheckResult { IsSpam = true, DeletePrevious = true };
        }

        return new SpamCheckResult { IsBlocked = false, IsSpam = false };
    }

    // ========== ДЛЯ ПРОВЕРКИ БЕЗ КОНТЕНТА ==========
    public async Task<bool> CheckSpamAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return true;

        if (await _userManager.IsLockedOutAsync(user))
            return true;

        return false;
    }

    // ========== ОСТАЛЬНЫЕ МЕТОДЫ ==========
    public async Task<bool> IsSpammerAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        return false;
    }

    public async Task<bool> IsIpBlockedAsync(string ip)
    {
        return false;
    }
}

// ========== КЛАСС ДЛЯ РЕЗУЛЬТАТА ==========
public class SpamCheckResult
{
    public bool IsBlocked { get; set; }
    public bool IsSpam { get; set; }
    public string Message { get; set; } = "";
    public bool DeletePrevious { get; set; }
}
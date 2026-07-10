using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class AdminService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public AdminService(AppDbContext db, UserManager<AppUser> userManager, IServiceScopeFactory scopeFactory)
    {
        _db = db;
        _userManager = userManager;
        _scopeFactory = scopeFactory;
    }

    // ========== ФИКС ВРЕМЕНИ ==========
    public static string FormatLocalTime(DateTime utcTime)
    {
        var localTime = utcTime.AddHours(3);
        return localTime.ToString("dd.MM.yyyy HH:mm");
    }

    // ========== ПОЛЬЗОВАТЕЛИ ==========

    public async Task<List<AppUser>> GetUsersAsync() =>
        await _db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

    public async Task<AppUser?> GetUserByIdAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;
        return await _db.Users.FindAsync(userId);
    }

    public async Task<int> GetTotalUsersAsync() =>
        await _db.Users.CountAsync();

    public async Task<List<AppUser>> SearchUsersAsync(string query)
    {
        return await _db.Users
            .Where(u => u.UserName != null && u.UserName.Contains(query))
            .OrderByDescending(u => u.CreatedAt)
            .Take(20)
            .ToListAsync();
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, "Admin");
    }

    public async Task<bool> IsSuperAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName == "admin";
    }

    // ========== УПРАВЛЕНИЕ АДМИНАМИ ==========

    public async Task<bool> ToggleAdminAsync(string userId, string currentUserId)
    {
        if (!await IsSuperAdminAsync(currentUserId)) return false;
        if (userId == currentUserId) return false;
        if (await IsSuperAdminAsync(userId)) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentUser = await _userManager.FindByIdAsync(currentUserId);
        var adminName = currentUser?.UserName ?? "";

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            _db.AdminLogs.Add(new AdminLog
            {
                AdminId = currentUserId,
                AdminName = adminName,
                Action = "Снятие админа",
                TargetUserId = userId,
                TargetUserName = user.UserName ?? "",
                Details = ""
            });
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            _db.AdminLogs.Add(new AdminLog
            {
                AdminId = currentUserId,
                AdminName = adminName,
                Action = "Выдача админа",
                TargetUserId = userId,
                TargetUserName = user.UserName ?? "",
                Details = ""
            });
        }
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<AppUser>> GetAdminsAsync() =>
        (await _userManager.GetUsersInRoleAsync("Admin")).ToList();

    // ========== УДАЛЕНИЕ ==========

    public async Task<(bool Success, string Error)> DeleteUserAsync(string userId, string currentUserId)
    {
        if (userId == currentUserId) return (false, "Нельзя удалить самого себя");
        if (await IsSuperAdminAsync(userId)) return (false, "Нельзя удалить супер-админа");
        if (!await IsSuperAdminAsync(currentUserId) && await IsAdminAsync(userId))
            return (false, "Нельзя удалить администратора");

        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var adminName = currentUser?.UserName ?? "";
            _db.AdminLogs.Add(new AdminLog
            {
                AdminId = currentUserId,
                AdminName = adminName,
                Action = "Удаление пользователя",
                TargetUserId = userId,
                TargetUserName = user.UserName ?? "",
                Details = ""
            });
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, "");
        }
        return (false, "Пользователь не найден");
    }

    // ========== РЕДАКТИРОВАНИЕ ==========

    public async Task<(bool Success, string Error)> UpdateUserDisplayNameAsync(string userId, string? displayName, string currentUserId)
    {
        if (await IsSuperAdminAsync(userId) && !await IsSuperAdminAsync(currentUserId))
            return (false, "Нельзя редактировать супер-админа");
        if (!await IsSuperAdminAsync(currentUserId) && await IsAdminAsync(userId) && userId != currentUserId)
            return (false, "Нельзя редактировать администратора");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return (false, "Пользователь не найден");

        var oldName = user.DisplayName;
        user.DisplayName = displayName;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            _db.AdminLogs.Add(new AdminLog
            {
                AdminId = currentUserId,
                AdminName = currentUser?.UserName ?? "",
                Action = "Изменение профиля",
                TargetUserId = userId,
                TargetUserName = user.UserName ?? "",
                Details = $"DisplayName изменён: {oldName} → {displayName}"
            });
            await _db.SaveChangesAsync();
            return (true, "Профиль обновлён");
        }
        return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // ========== СБРОС ПАРОЛЯ ==========

    public async Task<(bool Success, string Error)> ResetUserPasswordAsync(string userId, string newPassword, string currentUserId)
    {
        if (await IsSuperAdminAsync(userId) && !await IsSuperAdminAsync(currentUserId))
            return (false, "Нельзя сбросить пароль супер-админа");
        if (!await IsSuperAdminAsync(currentUserId) && await IsAdminAsync(userId) && userId != currentUserId)
            return (false, "Нельзя сбросить пароль администратора");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return (false, "Пользователь не найден");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            _db.AdminLogs.Add(new AdminLog
            {
                AdminId = currentUserId,
                AdminName = currentUser?.UserName ?? "",
                Action = "Сброс пароля",
                TargetUserId = userId,
                TargetUserName = user.UserName ?? "",
                Details = "Пароль сброшен администратором"
            });
            await _db.SaveChangesAsync();
            return (true, "Пароль сброшен");
        }
        return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // ========== БЛОКИРОВКА ==========

    public async Task ToggleLockoutAsync(string userId, string adminId, string adminName, string reason)
    {
        if (await IsSuperAdminAsync(userId))
            return;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return;

        if (await _userManager.IsLockedOutAsync(user))
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _db.SaveChangesAsync();
            return;
        }

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

        _db.BanRecords.Add(new BanRecord
        {
            UserId = userId,
            AdminId = adminId,
            AdminName = adminName,
            Reason = reason,
            BannedAt = DateTime.UtcNow
        });

        _db.AdminLogs.Add(new AdminLog
        {
            AdminId = adminId,
            AdminName = adminName,
            Action = "Блокировка",
            TargetUserId = userId,
            TargetUserName = user.UserName ?? "",
            Details = reason
        });

        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsLockedOutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsLockedOutAsync(user);
    }

    public async Task<BanRecord?> GetActiveBanAsync(string userId)
    {
        return await _db.BanRecords.FirstOrDefaultAsync(b => b.UserId == userId && b.UnbannedAt == null);
    }

    public async Task<BanRecordDisplay?> GetActiveBanDisplayAsync(string userId)
    {
        var ban = await _db.BanRecords
            .Where(b => b.UserId == userId && b.UnbannedAt == null)
            .OrderByDescending(b => b.BannedAt)
            .FirstOrDefaultAsync();

        if (ban == null) return null;

        return new BanRecordDisplay
        {
            UserId = ban.UserId,
            AdminId = ban.AdminId,
            AdminName = ban.AdminName,
            Reason = ban.Reason,
            BannedAt = ban.BannedAt,
            BannedAtLocal = FormatLocalTime(ban.BannedAt),
            UnbannedAt = ban.UnbannedAt
        };
    }

    // ========== АПЕЛЛЯЦИИ ==========

    public async Task AddBanAppealAsync(string userId, string userName, string banReason, string adminName, string appealText)
    {
        _db.BanAppeals.Add(new BanAppeal
        {
            UserId = userId,
            UserName = userName,
            BanReason = banReason,
            AdminName = adminName,
            AppealText = appealText,
            CreatedAt = DateTime.UtcNow,
            Status = BanAppealStatus.NotReviewed
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<BanAppeal>> GetBanAppealsAsync()
    {
        return await _db.BanAppeals
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveAppealAsync(int appealId)
    {
        var appeal = await _db.BanAppeals.FindAsync(appealId);
        if (appeal == null || appeal.Status != BanAppealStatus.NotReviewed) return;

        var user = await _userManager.FindByIdAsync(appeal.UserId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);

            var activeBans = await _db.BanRecords
                .Where(b => b.UserId == appeal.UserId && b.UnbannedAt == null)
                .ToListAsync();
            foreach (var ban in activeBans)
                ban.UnbannedAt = DateTime.UtcNow;
        }

        appeal.Status = BanAppealStatus.ReviewedUnbanned;
        await _db.SaveChangesAsync();
    }

    public async Task RejectAppealAsync(int appealId)
    {
        var appeal = await _db.BanAppeals.FindAsync(appealId);
        if (appeal == null || appeal.Status != BanAppealStatus.NotReviewed) return;

        appeal.Status = BanAppealStatus.ReviewedRejected;
        await _db.SaveChangesAsync();
    }

    // ========== ЖУРНАЛ ==========

    public async Task<List<AdminLog>> GetLogsAsync(int count = 50)
    {
        return await _db.AdminLogs.OrderByDescending(l => l.Timestamp).Take(count).ToListAsync();
    }

    // ========== ПРОГРЕСС УРОКОВ ==========

    public async Task<List<LessonProgress>> GetUserProgressAsync(string userId)
    {
        return await _db.LessonProgresses
            .Where(lp => lp.UserId == userId)
            .OrderBy(lp => lp.LessonId)
            .ToListAsync();
    }

    public static string GetLessonName(string lessonId) => lessonId switch
    {
        "data-is-oil" => "1. Кто ты в мире анонимности?",
        "browser-wars" => "2. Твой браузер — защитник",
        "vpn-mask" => "3. VPN — первый щит",
        "onion-layer" => "4. Tor и луковая маршрутизация",
        "darknet" => "5. Мифы и правда о даркнете",
        "pgp" => "6. PGP — шифрование",
        "simswap" => "7. SIM-swap защита",
        "phishing" => "8. Фишинг — видеть обман",
        "crypto-lesson" => "9. Криптовалюта и анонимность",
        "opsec" => "10. Твоя цифровая крепость",
        _ => lessonId
    };

    // ========== НОВОСТИ ==========

    public async Task<List<NewsItem>> GetAllNewsAsync() =>
        await _db.NewsItems.OrderByDescending(n => n.PublishedAt).ToListAsync();

    public async Task AddNewsAsync(NewsItem news)
    {
        news.PublishedAt = DateTime.UtcNow;
        _db.NewsItems.Add(news);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateNewsAsync(int id, string title, string summary, string source, string category)
    {
        var news = await _db.NewsItems.FindAsync(id);
        if (news != null)
        {
            news.Title = title;
            news.Summary = summary;
            news.Source = source;
            news.Category = category;
            _db.NewsItems.Update(news);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteNewsAsync(int id)
    {
        var n = await _db.NewsItems.FindAsync(id);
        if (n != null)
        {
            _db.NewsItems.Remove(n);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteAllNewsAsync()
    {
        await _db.NewsItems.ExecuteDeleteAsync();
        await _db.SaveChangesAsync();
    }

    // ========== RSS ==========

    public async Task<string> ForceRssUpdateAsync()
    {
        try
        {
            var rss = new RssBackgroundService(_scopeFactory);
            await rss.ForceUpdateAsync();
            return "RSS обновлён!";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    // ========== СТАТИСТИКА ==========

    public async Task<AdminStats> GetStatsAsync() => new()
    {
        TotalUsers = await _db.Users.CountAsync(),
        TotalNews = await _db.NewsItems.CountAsync(),
        TotalLessonsCompleted = await _db.LessonProgresses.CountAsync(lp => lp.Completed),
        TotalProfiles = await _db.UserProfiles.CountAsync()
    };

    public async Task<List<DailyStats>> GetDailyStatsAsync()
    {
        var result = new List<DailyStats>();
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var count = await _db.Users.CountAsync(u => u.CreatedAt.Date == date);
            result.Add(new DailyStats { Date = date, Registrations = count });
        }
        return result;
    }

    public async Task<List<UserProgress>> GetTopUsersAsync(int count = 10)
    {
        var users = await _db.Users.ToListAsync();
        var result = new List<UserProgress>();
        foreach (var user in users)
        {
            var completed = await _db.LessonProgresses.CountAsync(lp => lp.UserId == user.Id && lp.Completed);
            result.Add(new UserProgress { UserName = user.UserName ?? "?", CompletedLessons = completed });
        }
        return result.OrderByDescending(u => u.CompletedLessons).Take(count).ToList();
    }
}

// ========== DTO ==========

public class AdminStats
{
    public int TotalUsers { get; set; }
    public int TotalNews { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public int TotalProfiles { get; set; }
}

public class UserProgress
{
    public string UserName { get; set; } = "";
    public int CompletedLessons { get; set; }
}

public class DailyStats
{
    public DateTime Date { get; set; }
    public int Registrations { get; set; }
}

public class BanRecordDisplay
{
    public string UserId { get; set; } = "";
    public string AdminId { get; set; } = "";
    public string AdminName { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime BannedAt { get; set; }
    public string BannedAtLocal { get; set; } = "";
    public DateTime? UnbannedAt { get; set; }
}
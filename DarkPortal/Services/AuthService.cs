using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AntiSpamService _antiSpam;

    public AuthService(
        AppDbContext db,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AntiSpamService antiSpam)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _antiSpam = antiSpam;
    }

    // ========== РЕГИСТРАЦИЯ ==========

    public async Task<(bool Success, string Error)> Register(string username, string password, string ip)
    {
        // Проверка на спам-регистрации
        if (!_antiSpam.CanRegister(ip))
            return (false, "Слишком много регистраций с этого IP. Подождите немного.");

        var existingUser = await _userManager.FindByNameAsync(username);
        if (existingUser != null)
            return (false, "Пользователь с таким именем уже существует.");

        var user = new AppUser
        {
            UserName = username,
            DisplayName = username,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Ошибка регистрации: {errors}");
        }

        // Первый пользователь = Admin, остальные = User
        var totalUsers = await _db.Users.CountAsync();
        if (totalUsers == 1)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        await _db.SaveChangesAsync();
        return (true, "");
    }

    // ========== ВХОД ==========

    public async Task<(bool Success, string Error, AppUser? User)> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return (false, "Пользователь не найден", null);

        if (await _userManager.IsLockedOutAsync(user))
        {
            var ban = await _db.BanRecords.FirstOrDefaultAsync(b => b.UserId == user.Id && b.UnbannedAt == null);
            var reason = ban?.Reason ?? "Не указана";
            return (false, $"Ваш аккаунт заблокирован. Причина: {reason}", user);
        }

        var result = await _signInManager.PasswordSignInAsync(username, password, true, false);
        if (!result.Succeeded)
            return (false, "Неверный пароль", null);

        return (true, "", user);
    }

    // ========== ВЫХОД ==========

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    // ========== ПРОВЕРКА АУТЕНТИФИКАЦИИ ==========

    public async Task<AppUser?> GetCurrentUserAsync()
    {
        var user = await _userManager.GetUserAsync(_signInManager.Context.User);
        return user;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return _signInManager.Context.User?.Identity?.IsAuthenticated ?? false;
    }

    public async Task<bool> IsAdminAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return false;
        return await _userManager.IsInRoleAsync(user, "Admin");
    }

    public async Task<bool> IsSuperAdminAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return false;
        return user.UserName == "admin";
    }

    // ========== ВОССТАНОВЛЕНИЕ ПАРОЛЯ ==========

    public async Task<(bool Success, string Error)> ResetPasswordAsync(string username, string recoveryCode, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return (false, "Пользователь не найден");

        if (user.RecoveryCode != recoveryCode.ToUpper())
            return (false, "Неверный код восстановления");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
        {
            user.RecoveryCode = Guid.NewGuid().ToString("N")[..12].ToUpper();
            await _userManager.UpdateAsync(user);
            return (true, "");
        }

        return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // ========== СМЕНА ПАРОЛЯ ==========

    public async Task<(bool Success, string Error)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "Пользователь не найден");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
            return (true, "");

        return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // ========== ГЕНЕРАЦИЯ КОДА ВОССТАНОВЛЕНИЯ ==========

    public async Task<string> GenerateRecoveryCodeAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return "";

        var code = Guid.NewGuid().ToString("N")[..12].ToUpper();
        user.RecoveryCode = code;
        await _userManager.UpdateAsync(user);
        return code;
    }

    // ========== ПОЛУЧЕНИЕ ИНФОРМАЦИИ ==========

    public async Task<AppUser?> GetUserByNameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<AppUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    // ========== ОБНОВЛЕНИЕ ПРОФИЛЯ ==========

    public async Task<(bool Success, string Error)> UpdateProfileAsync(string userId, string? displayName, string? email, string? avatarUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "Пользователь не найден");

        bool hasChanges = false;

        if (!string.IsNullOrEmpty(displayName) && user.DisplayName != displayName)
        {
            user.DisplayName = displayName;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            user.Email = email;
            hasChanges = true;
        }

        if (avatarUrl != null && user.AvatarUrl != avatarUrl)
        {
            user.AvatarUrl = avatarUrl;
            hasChanges = true;
        }

        if (hasChanges)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return (true, "Профиль обновлён");
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return (true, "Изменений не было");
    }

    // ========== СТАТУС БАНА ==========

    public async Task<(bool IsBanned, string Reason, string AdminName, DateTime BannedAt)> GetBanStatusAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "", "", DateTime.UtcNow);

        if (!await _userManager.IsLockedOutAsync(user))
            return (false, "", "", DateTime.UtcNow);

        var ban = await _db.BanRecords.FirstOrDefaultAsync(b => b.UserId == userId && b.UnbannedAt == null);
        if (ban == null)
            return (true, "Не указана", "Система", DateTime.UtcNow);

        return (true, ban.Reason, ban.AdminName, ban.BannedAt);
    }

    // ========== ДЛЯ АДМИНКИ ==========
    public async Task<string> GetCurrentUserIdAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Id ?? "";
    }

    public async Task<string> GetCurrentUserNameAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.UserName ?? "";
    }
}
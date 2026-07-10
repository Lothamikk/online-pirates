using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class ProfileService
{
    private readonly AppDbContext _db;
    private readonly AuthenticationStateProvider _auth;
    private readonly LocalStorageService _storage;
    private const string GuestKey = "user_profile_guest";

    public UserDataProfile CurrentProfile { get; private set; } = new();
    public event Action? OnProfileChanged;
    public int UserLevel { get; set; } = 1;

    public ProfileService(AppDbContext db, AuthenticationStateProvider auth, LocalStorageService storage)
    {
        _db = db;
        _auth = auth;
        _storage = storage;
    }

    public async Task LoadProfileAsync()
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null)
            {
                CurrentProfile = profile;
                OnProfileChanged?.Invoke();
                return;
            }
        }

        try
        {
            var json = await _storage.GetItem(GuestKey);
            if (!string.IsNullOrEmpty(json))
            {
                CurrentProfile = System.Text.Json.JsonSerializer.Deserialize<UserDataProfile>(json) ?? new();
            }
        }
        catch { }

        OnProfileChanged?.Invoke();
    }

    public async Task UpdateProfile(UserDataProfile newProfile)
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            var existing = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existing != null)
            {
                existing.Name = newProfile.Name;
                existing.Age = newProfile.Age;
                existing.HasDog = newProfile.HasDog;
                existing.HasCar = newProfile.HasCar;
                existing.HasSmartHome = newProfile.HasSmartHome;
                existing.UsesVPN = newProfile.UsesVPN;
                _db.UserProfiles.Update(existing);
            }
            else
            {
                newProfile.UserId = userId;
                _db.UserProfiles.Add(newProfile);
            }
            await _db.SaveChangesAsync();
            CurrentProfile = existing ?? newProfile;
        }
        else
        {
            CurrentProfile = newProfile;
            var json = System.Text.Json.JsonSerializer.Serialize(newProfile);
            await _storage.SetItem(GuestKey, json);
        }

        OnProfileChanged?.Invoke();
    }

    public async Task<AppUser?> GetUserProfileAsync(string userId)
    {
        return await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateBioAsync(string userId, string bio)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.Bio = bio; await _db.SaveChangesAsync(); }
    }

    public async Task UpdateBannerAsync(string userId, string bannerUrl)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.BannerUrl = bannerUrl; await _db.SaveChangesAsync(); }
    }

    public async Task<List<ProfileComment>> GetCommentsAsync(string profileUserId)
    {
        return await _db.ProfileComments
            .Where(c => c.ProfileUserId == profileUserId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddCommentAsync(string profileUserId, string authorId, string authorName, string content)
    {
        _db.ProfileComments.Add(new ProfileComment
        {
            ProfileUserId = profileUserId,
            AuthorId = authorId,
            AuthorName = authorName,
            Content = content
        });
        await _db.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _db.ProfileComments.FindAsync(commentId);
        if (comment != null)
        {
            _db.ProfileComments.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateDisplayNameAsync(string userId, string displayName)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.DisplayName = displayName; await _db.SaveChangesAsync(); }
    }

    public async Task UpdateAvatarAsync(string userId, string avatarUrl)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.AvatarUrl = avatarUrl; await _db.SaveChangesAsync(); }
    }

    public async Task UpdatePinnedBadgeAsync(string userId, string badgeId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.PinnedBadge = badgeId; await _db.SaveChangesAsync(); }
    }
}
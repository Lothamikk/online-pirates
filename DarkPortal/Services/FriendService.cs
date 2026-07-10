using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class FriendService
{
    private readonly AppDbContext _db;

    public FriendService(AppDbContext db) => _db = db;

    public async Task<bool> IsFriend(string userId, string friendId)
    {
        return await _db.Friendships.AnyAsync(f =>
            f.Accepted && ((f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)));
    }

    public async Task<bool> HasPendingRequest(string userId, string friendId)
    {
        return await _db.Friendships.AnyAsync(f =>
            !f.Accepted && f.UserId == userId && f.FriendId == friendId);
    }

    public async Task SendRequest(string userId, string friendId)
    {
        if (await HasPendingRequest(userId, friendId)) return;
        if (await IsFriend(userId, friendId)) return;

        _db.Friendships.Add(new Friendship { UserId = userId, FriendId = friendId });
        await _db.SaveChangesAsync();
    }

    public async Task AcceptRequest(string userId, string friendId)
    {
        var request = await _db.Friendships.FirstOrDefaultAsync(f =>
            !f.Accepted && f.UserId == friendId && f.FriendId == userId);
        if (request != null) { request.Accepted = true; await _db.SaveChangesAsync(); }
    }

    public async Task RemoveFriend(string userId, string friendId)
    {
        var friendships = await _db.Friendships.Where(f =>
            (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)).ToListAsync();
        _db.Friendships.RemoveRange(friendships);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AppUser>> GetFriends(string userId)
    {
        var friendIds = await _db.Friendships
            .Where(f => f.Accepted && (f.UserId == userId || f.FriendId == userId))
            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
            .Distinct()
            .ToListAsync();

        return await _db.Users.Where(u => friendIds.Contains(u.Id)).ToListAsync();
    }

    public async Task<List<AppUser>> GetPendingRequests(string userId)
    {
        var requesterIds = await _db.Friendships
            .Where(f => !f.Accepted && f.FriendId == userId)
            .Select(f => f.UserId)
            .ToListAsync();

        return await _db.Users.Where(u => requesterIds.Contains(u.Id)).ToListAsync();
    }

    public async Task<int> GetFriendCount(string userId)
    {
        return await _db.Friendships
            .CountAsync(f => f.Accepted && (f.UserId == userId || f.FriendId == userId));
    }
}
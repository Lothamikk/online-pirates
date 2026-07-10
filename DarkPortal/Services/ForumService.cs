using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class ForumService
{
    private readonly AppDbContext _db;
    private readonly AdminService _adminService;

    public ForumService(AppDbContext db, AdminService adminService)
    {
        _db = db;
        _adminService = adminService;
    }

    public async Task<bool> IsAdminAsync(string userId) => await _adminService.IsAdminAsync(userId);
    public async Task<bool> IsSuperAdminAsync(string userId) => await _adminService.IsSuperAdminAsync(userId);

    public async Task<List<ForumSection>> GetSectionsAsync()
    {
        if (!await _db.ForumSections.AnyAsync())
        {
            _db.ForumSections.Add(new ForumSection { Name = "Общее", Order = 1 });
            _db.ForumSections.Add(new ForumSection { Name = "Кибербезопасность", Order = 2 });
            _db.ForumSections.Add(new ForumSection { Name = "Анонимность", Order = 3 });
            await _db.SaveChangesAsync();
        }
        return await _db.ForumSections.OrderBy(s => s.Order).ToListAsync();
    }

    public async Task AddSectionAsync(string name)
    {
        _db.ForumSections.Add(new ForumSection { Name = name, Order = await _db.ForumSections.CountAsync() + 1 });
        await _db.SaveChangesAsync();
    }

    public async Task<List<ForumTopic>> GetTopicsAsync(string? section = null)
    {
        var query = _db.ForumTopics.Include(t => t.Replies).AsQueryable();
        if (!string.IsNullOrEmpty(section)) query = query.Where(t => t.Section == section);
        return await query.OrderByDescending(t => t.IsPinned).ThenByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<ForumTopic?> GetTopicAsync(int id)
    {
        return await _db.ForumTopics.Include(t => t.Replies.OrderBy(r => r.CreatedAt)).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ForumTopic> CreateTopicAsync(string title, string content, string authorId, string authorName, string section = "Общее")
    {
        var topic = new ForumTopic { Title = title, Content = content, AuthorId = authorId, AuthorName = authorName, Section = section };
        _db.ForumTopics.Add(topic); await _db.SaveChangesAsync(); return topic;
    }

    public async Task TogglePinAsync(int topicId)
    {
        var topic = await _db.ForumTopics.FindAsync(topicId);
        if (topic != null) { topic.IsPinned = !topic.IsPinned; await _db.SaveChangesAsync(); }
    }

    public async Task DeleteTopicAsync(int id)
    {
        var topic = await _db.ForumTopics.Include(t => t.Replies).FirstOrDefaultAsync(t => t.Id == id);
        if (topic != null) { _db.ForumReplies.RemoveRange(topic.Replies); _db.ForumTopics.Remove(topic); await _db.SaveChangesAsync(); }
    }

    public async Task<ForumReply> AddReplyAsync(int topicId, string content, string authorId, string authorName)
    {
        var reply = new ForumReply { TopicId = topicId, Content = content, AuthorId = authorId, AuthorName = authorName };
        _db.ForumReplies.Add(reply); await _db.SaveChangesAsync(); return reply;
    }

    public async Task DeleteReplyAsync(int id)
    {
        var r = await _db.ForumReplies.FindAsync(id);
        if (r != null) { _db.ForumReplies.Remove(r); await _db.SaveChangesAsync(); }
    }

    public async Task<bool> CanCreateTopicAsync(string userId)
    {
        return !await _db.ForumTopics.AnyAsync(t => t.AuthorId == userId && t.CreatedAt > DateTime.UtcNow.AddMinutes(-2));
    }

    public async Task<int> GetCooldownSecondsAsync(string userId)
    {
        var last = await _db.ForumTopics.Where(t => t.AuthorId == userId).OrderByDescending(t => t.CreatedAt).FirstOrDefaultAsync();
        if (last == null) return 0;
        return Math.Max(0, 120 - (int)(DateTime.UtcNow - last.CreatedAt).TotalSeconds);
    }
    public async Task DeleteSectionAsync(int id)
    {
        var section = await _db.ForumSections.FindAsync(id);
        if (section != null) { _db.ForumSections.Remove(section); await _db.SaveChangesAsync(); }
    }
}
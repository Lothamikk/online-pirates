using Microsoft.EntityFrameworkCore;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class MessageService
{
    private readonly AppDbContext _db;

    public MessageService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SecretMessage> CreateMessage(string text, bool encrypt = false)
    {
        var message = new SecretMessage
        {
            Text = text,
            IsEncrypted = encrypt,
            CreatedAt = DateTime.UtcNow
        };

        _db.SecretMessages.Add(message);
        await _db.SaveChangesAsync();
        return message;
    }

    public async Task<SecretMessage?> GetMessage(string id)
    {
        var message = await _db.SecretMessages.FindAsync(id);

        if (message == null)
            return null;

        // Проверка на срок (24 часа)
        if (DateTime.UtcNow - message.CreatedAt > TimeSpan.FromHours(24))
        {
            _db.SecretMessages.Remove(message);
            await _db.SaveChangesAsync();
            return null;
        }

        // Если уже просмотрено — не показываем
        if (message.IsViewed)
            return null;

        return message;
    }

    public async Task MarkAsViewed(string id)
    {
        var message = await _db.SecretMessages.FindAsync(id);
        if (message != null)
        {
            message.ViewedAt = DateTime.UtcNow;
            _db.SecretMessages.Update(message);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteMessage(string id)
    {
        var message = await _db.SecretMessages.FindAsync(id);
        if (message != null)
        {
            _db.SecretMessages.Remove(message);
            await _db.SaveChangesAsync();
        }
    }

    public async Task CleanupOldMessages()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var old = await _db.SecretMessages
            .Where(m => m.CreatedAt < cutoff || m.IsViewed)
            .ToListAsync();

        _db.SecretMessages.RemoveRange(old);
        await _db.SaveChangesAsync();
    }
}
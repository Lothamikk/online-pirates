using System.Collections.Concurrent;

namespace DarkPortal.Services;

public class NotificationService
{
    private readonly ConcurrentDictionary<string, List<Notification>> _notifications = new();

    public event Action? OnChanged;

    public void AddNotification(string userId, Notification notification)
    {
        if (!_notifications.ContainsKey(userId))
            _notifications[userId] = new List<Notification>();
        _notifications[userId].Add(notification);
        OnChanged?.Invoke();
    }

    public List<Notification> GetNotifications(string userId)
    {
        return _notifications.GetValueOrDefault(userId, new List<Notification>());
    }

    public int GetUnreadCount(string userId)
    {
        return _notifications.GetValueOrDefault(userId, new List<Notification>()).Count(n => !n.IsRead);
    }

    public void MarkAsRead(string userId, string notificationId)
    {
        if (_notifications.TryGetValue(userId, out var list))
        {
            var n = list.FirstOrDefault(x => x.Id == notificationId);
            if (n != null) n.IsRead = true;
            OnChanged?.Invoke();
        }
    }

    public void ClearAll(string userId)
    {
        _notifications.TryRemove(userId, out _);
        OnChanged?.Invoke();
    }
}

public class Notification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Message { get; set; } = "";
    public string Link { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
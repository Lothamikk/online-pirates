using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class SecretMessage
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Text { get; set; } = "";
    public bool IsEncrypted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ViewedAt { get; set; }
    public bool IsViewed => ViewedAt != null;
}
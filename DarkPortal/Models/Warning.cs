using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class Warning
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive => DateTime.UtcNow < ExpiresAt;
}
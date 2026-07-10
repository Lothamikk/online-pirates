using System;

namespace DarkPortal.Models;

public class BanRecord
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string AdminId { get; set; } = "";
    public string AdminName { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime BannedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnbannedAt { get; set; }
}
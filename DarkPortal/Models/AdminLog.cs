using System;

namespace DarkPortal.Models;

public class AdminLog
{
    public int Id { get; set; }
    public string AdminId { get; set; } = "";
    public string AdminName { get; set; } = "";
    public string Action { get; set; } = "";
    public string TargetUserId { get; set; } = "";
    public string TargetUserName { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class Friendship
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string FriendId { get; set; } = "";
    public bool Accepted { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}
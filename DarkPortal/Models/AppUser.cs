using Microsoft.AspNetCore.Identity;

namespace DarkPortal.Models;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int LessonsCompleted { get; set; }
    public UserDataProfile? Profile { get; set; }
    public string? RecoveryCode { get; set; }
    public string? Bio { get; set; }
    public string? BannerUrl { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PinnedBadge { get; set; }
}
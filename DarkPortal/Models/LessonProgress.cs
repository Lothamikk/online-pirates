namespace DarkPortal.Models;

public class LessonProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string LessonId { get; set; } = "";
    public bool Completed { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public AppUser? User { get; set; }
}
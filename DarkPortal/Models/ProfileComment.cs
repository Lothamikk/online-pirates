using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class ProfileComment
{
    [Key]
    public int Id { get; set; }
    public string ProfileUserId { get; set; } = "";
    public string AuthorId { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
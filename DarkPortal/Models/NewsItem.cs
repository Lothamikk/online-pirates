using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class NewsItem
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Source { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime PublishedAt { get; set; }
    public string Severity { get; set; } = "Info";
    public string? Link { get; set; }
}
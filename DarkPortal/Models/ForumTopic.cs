using System.ComponentModel.DataAnnotations;

namespace DarkPortal.Models;

public class ForumTopic
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string AuthorId { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPinned { get; set; }
    public string Section { get; set; } = "Общее";
    public List<ForumReply> Replies { get; set; } = new();
}

public class ForumReply
{
    [Key]
    public int Id { get; set; }
    public int TopicId { get; set; }
    public string Content { get; set; } = "";
    public string AuthorId { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ForumTopic? Topic { get; set; }
}

public class ForumSection
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Order { get; set; }
}
namespace DarkPortal.Models;

public class Achievement
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Color { get; set; } = "Primary";
    public bool Unlocked { get; set; }
}
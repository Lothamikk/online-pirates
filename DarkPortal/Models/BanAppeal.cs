using System;

namespace DarkPortal.Models;

public class BanAppeal
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string BanReason { get; set; } = "";
    public string AdminName { get; set; } = "";
    public string AppealText { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public BanAppealStatus Status { get; set; } = BanAppealStatus.NotReviewed;

    // Совместимость со старым кодом
    public bool IsReviewed
    {
        get => Status != BanAppealStatus.NotReviewed;
        set => Status = value ? BanAppealStatus.ReviewedUnbanned : BanAppealStatus.NotReviewed;
    }
}
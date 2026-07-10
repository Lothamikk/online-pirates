using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DarkPortal.Models;

namespace DarkPortal.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserDataProfile> UserProfiles { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<NewsItem> NewsItems { get; set; }
    public DbSet<SecretMessage> SecretMessages { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<ForumReply> ForumReplies { get; set; }
    public DbSet<ForumSection> ForumSections { get; set; }
    public DbSet<ProfileComment> ProfileComments { get; set; }
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<BanRecord> BanRecords { get; set; }
    public DbSet<BanAppeal> BanAppeals { get; set; }
    public DbSet<AdminLog> AdminLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserDataProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<UserDataProfile>(p => p.UserId);

        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.User)
            .WithMany()
            .HasForeignKey(lp => lp.UserId);
    }
}
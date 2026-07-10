using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class NewsService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public NewsService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<NewsItem>> GetLatestNewsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.NewsItems
            .OrderByDescending(n => n.PublishedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task ClearAndReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Удаляем все новости
        await db.NewsItems.ExecuteDeleteAsync();
        await db.SaveChangesAsync();
    }
}
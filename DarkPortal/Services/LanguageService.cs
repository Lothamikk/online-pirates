using DarkPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DarkPortal.Services;

public class LanguageService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public string CurrentLanguage { get; private set; } = "ru";
    public event Action? OnLanguageChanged;

    public LanguageService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void SetLanguage(string lang)
    {
        CurrentLanguage = lang;

        Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.NewsItems.ExecuteDeleteAsync();
        });

        OnLanguageChanged?.Invoke();
    }
}
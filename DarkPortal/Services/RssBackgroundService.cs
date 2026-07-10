using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DarkPortal.Data;
using DarkPortal.Models;

namespace DarkPortal.Services;

public class RssBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly List<(string Url, string Source, string Category)> Feeds = new()
    {
        ("https://feeds.feedburner.com/TheHackersNews", "The Hacker News", "Угрозы"),
        ("https://www.bleepingcomputer.com/feed/", "BleepingComputer", "Уязвимости"),
        ("https://krebsonsecurity.com/feed/", "Krebs on Security", "Утечки"),
    };

    public RssBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(3000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await FetchAllFeeds(); }
            catch { }
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    public async Task ForceUpdateAsync()
    {
        await FetchAllFeeds();
    }

    private async Task FetchAllFeeds()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var translator = scope.ServiceProvider.GetRequiredService<TranslationService>();
        var langService = scope.ServiceProvider.GetRequiredService<LanguageService>();
        var lang = langService.CurrentLanguage;

        foreach (var (url, source, category) in Feeds)
        {
            try
            {
                using var reader = XmlReader.Create(url);
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items.Take(5))
                {
                    var title = item.Title.Text;
                    var exists = await db.NewsItems.AnyAsync(n => n.Title == title);
                    if (exists) continue;

                    var summary = TruncateHtml(item.Summary?.Text ?? "", 200);
                    var titleFinal = lang == "en" ? title : await translator.TranslateAsync(title, "en", lang);

                    var news = new NewsItem
                    {
                        Title = titleFinal,
                        Summary = summary,
                        Source = source,
                        Category = TranslateCategory(category, lang),
                        PublishedAt = item.PublishDate.UtcDateTime,
                        Link = item.Links.FirstOrDefault()?.Uri?.ToString() ?? "",
                        Severity = category switch { "Утечки" => "Critical", "Уязвимости" => "Warning", _ => "Info" }
                    };

                    db.NewsItems.Add(news);
                }
            }
            catch { }
        }

        await db.SaveChangesAsync();
    }

    private string TruncateHtml(string html, int max)
    {
        if (string.IsNullOrEmpty(html)) return "";
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        return text.Length <= max ? text : text[..max] + "...";
    }

    private string TranslateCategory(string cat, string lang) => lang switch
    {
        "en" => cat switch { "Утечки" => "Leaks", "Угрозы" => "Threats", "Уязвимости" => "Vulnerabilities", _ => cat },
        "de" => cat switch { "Утечки" => "Lecks", "Угрозы" => "Bedrohungen", "Уязвимости" => "Schwachstellen", _ => cat },
        "fr" => cat switch { "Утечки" => "Fuite", "Угрозы" => "Menaces", "Уязвимости" => "Vulnérabilités", _ => cat },
        "es" => cat switch { "Утечки" => "Fugas", "Угрозы" => "Amenazas", "Уязвимости" => "Vulnerabilidades", _ => cat },
        _ => cat
    };
}
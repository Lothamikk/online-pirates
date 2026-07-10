using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Json;

namespace DarkPortal.Services;

public class HibpService
{
    private readonly HttpClient _http;

    public HibpService(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.Add("User-Agent", "DarkPortal");
    }

    public async Task<PasswordResult> CheckPasswordAsync(string password)
    {
        var result = new PasswordResult();

        // HaveIBeenPwned
        try
        {
            var sha1 = SHA1.HashData(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToHexString(sha1);
            var prefix = hash[..5];
            var suffix = hash[5..];

            var response = await _http.GetStringAsync($"https://api.pwnedpasswords.com/range/{prefix}");

            foreach (var line in response.Split('\n'))
            {
                var parts = line.Trim().Split(':');
                if (parts.Length == 2 && parts[0] == suffix)
                {
                    result.PwnedCount = int.Parse(parts[1]);
                    break;
                }
            }
        }
        catch { }

        // Анализ пароля
        result.Score = AnalyzePassword(password);
        result.Recommendations = GenerateRecommendations(password, result.PwnedCount);

        return result;
    }

    public async Task<EmailResult> CheckEmailAsync(string email)
    {
        var result = new EmailResult();

        // Пробуем Firefox Monitor (бесплатный)
        try
        {
            var url = $"https://monitor.firefox.com/api/v1/scan?email={Uri.EscapeDataString(email)}";
            var response = await _http.GetFromJsonAsync<FirefoxResponse>(url);

            if (response?.Breaches != null && response.Breaches.Count > 0)
            {
                result.Breaches = response.Breaches.Select(b => new BreachDetail
                {
                    Name = b.Name,
                    Date = b.AddedDate?.ToString("dd.MM.yyyy") ?? "?",
                    Description = b.Description ?? "",
                    DataClasses = b.DataClasses ?? new List<string>()
                }).ToList();
            }
        }
        catch
        {
            // Пробуем HaveIBeenPwned
            try
            {
                var response = await _http.GetFromJsonAsync<List<HibpBreach>>(
                    $"https://haveibeenpwned.com/api/v3/breachedaccount/{Uri.EscapeDataString(email)}?truncateResponse=true");

                if (response != null && response.Count > 0)
                {
                    result.Breaches = response.Select(b => new BreachDetail
                    {
                        Name = b.Name,
                        Date = b.BreachDate ?? "?",
                        Description = b.Description ?? "",
                        DataClasses = b.DataClasses ?? new List<string>()
                    }).ToList();
                }
            }
            catch { }
        }

        result.Recommendations = GenerateEmailRecommendations(result.Breaches);
        return result;
    }

    private int AnalyzePassword(string password)
    {
        int score = 0;
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Length >= 16) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
        return score;
    }

    private List<string> GenerateRecommendations(string password, int pwnedCount)
    {
        var tips = new List<string>();

        if (pwnedCount > 0)
        {
            tips.Add($"⚠️ Этот пароль найден в утечках {pwnedCount:N0} раз. Немедленно смени его!");
        }

        if (password.Length < 12)
            tips.Add("📏 Пароль короткий. Минимум 12 символов.");

        if (!password.Any(char.IsUpper))
            tips.Add("🔤 Добавь заглавные буквы (A-Z).");

        if (!password.Any(char.IsDigit))
            tips.Add("🔢 Добавь цифры (0-9).");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            tips.Add("🔣 Добавь спецсимволы (!@#$%).");

        if (password.Distinct().Count() < password.Length / 2)
            tips.Add("🔁 Слишком много повторяющихся символов.");

        if (tips.Count == 0)
            tips.Add("✅ Пароль выглядит надёжным!");

        return tips;
    }

    private List<string> GenerateEmailRecommendations(List<BreachDetail> breaches)
    {
        var tips = new List<string>();

        if (breaches.Count > 0)
        {
            tips.Add($"⚠️ Email найден в {breaches.Count} утечках!");
            tips.Add("🔑 Немедленно смени пароль на всех сервисах, где использовался этот email.");
            tips.Add("🔐 Включи двухфакторную аутентификацию (2FA) везде, где возможно.");
            tips.Add("📧 Используй разные email для разных сервисов.");
            tips.Add("🛡️ Рекомендуем использовать менеджер паролей.");
        }
        else
        {
            tips.Add("✅ Email не найден в известных утечках.");
            tips.Add("💡 Продолжай следить за безопасностью.");
        }

        return tips;
    }

    // Модели
    public class PasswordResult
    {
        public int PwnedCount { get; set; }
        public int Score { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public bool IsPwned => PwnedCount > 0;
    }

    public class EmailResult
    {
        public List<BreachDetail> Breaches { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public bool IsPwned => Breaches.Count > 0;
    }

    public class BreachDetail
    {
        public string Name { get; set; } = "";
        public string Date { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> DataClasses { get; set; } = new();
    }

    private class FirefoxResponse
    {
        public List<FirefoxBreach>? Breaches { get; set; }
    }

    private class FirefoxBreach
    {
        public string Name { get; set; } = "";
        public DateTime? AddedDate { get; set; }
        public string? Description { get; set; }
        public List<string>? DataClasses { get; set; }
    }

    private class HibpBreach
    {
        public string Name { get; set; } = "";
        public string? BreachDate { get; set; }
        public string? Description { get; set; }
        public List<string>? DataClasses { get; set; }
    }
}
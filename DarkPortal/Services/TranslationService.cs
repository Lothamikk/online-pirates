using System.Net.Http.Json;
using System.Text.Json;

namespace DarkPortal.Services;

public class TranslationService
{
    private readonly HttpClient _http;

    public TranslationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> TranslateAsync(string text, string fromLang = "en", string toLang = "ru")
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        if (fromLang == toLang) return text;

        try
        {
            var url = $"https://lingva.ml/api/v1/{fromLang}/{toLang}/{Uri.EscapeDataString(text)}";
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (json.TryGetProperty("translation", out var translation))
                {
                    return translation.GetString() ?? text;
                }
            }
        }
        catch { }

        return text;
    }
}
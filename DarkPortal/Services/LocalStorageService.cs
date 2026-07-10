using Microsoft.JSInterop;
using System.Text.Json;

namespace DarkPortal.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // ========== GET С GENERIC ==========
    public async Task<T?> GetItem<T>(string key)
    {
        try
        {
            var data = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(data))
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }
        catch
        {
            return default;
        }
    }

    // ========== GET БЕЗ GENERIC (ДЛЯ СТРОК) ==========
    public async Task<string> GetItem(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key) ?? "";
        }
        catch
        {
            return "";
        }
    }

    // ========== SET ==========
    public async Task SetItem<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    // ========== REMOVE ==========
    public async Task RemoveItem(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    // ========== CLEAR ==========
    public async Task Clear()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    // ========== CONTAINS ==========
    public async Task<bool> ContainsKey(string key)
    {
        var value = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return value != null;
    }
}
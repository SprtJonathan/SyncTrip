using System.Text;
using System.Text.Json;
using SyncTrip.App.Core.Platform;

namespace SyncTrip.App.Navigation;

public class DesktopSecureStorageService : ISecureStorageService
{
    private readonly string _filePath;
    private Dictionary<string, string> _store;

    public DesktopSecureStorageService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, "SyncTrip");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "storage.json");
        _store = Load();
    }

    public Task<string?> GetAsync(string key)
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string key, string value)
    {
        _store[key] = value;
        Save();
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _store.Remove(key);
        Save();
    }

    private Dictionary<string, string> Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new Dictionary<string, string>();

            var json = File.ReadAllText(_filePath, Encoding.UTF8);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_store);
            File.WriteAllText(_filePath, json, Encoding.UTF8);
        }
        catch
        {
            // Fallback silencieux
        }
    }
}

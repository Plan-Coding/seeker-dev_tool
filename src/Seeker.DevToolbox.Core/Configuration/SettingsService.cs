using System.Text.Json;

namespace Seeker.DevToolbox.Core.Configuration;

/// <summary>
/// 读写设置文件。写入使用临时文件 + 覆盖移动,避免断电/崩溃留下半个 JSON。
/// </summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly AppPaths _paths;

    public SettingsService(AppPaths paths)
    {
        _paths = paths;
        Current = Load();
    }

    public AppSettings Current { get; }

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_paths.SettingsFile))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_paths.SettingsFile);
            return JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            BackupCorruptFile();
            return new AppSettings();
        }
    }

    public bool TrySave(out string? error)
    {
        try
        {
            var tempFile = _paths.SettingsFile + ".tmp";
            var json = JsonSerializer.Serialize(Current, SerializerOptions);
            File.WriteAllText(tempFile, json);
            File.Move(tempFile, _paths.SettingsFile, overwrite: true);
            error = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            error = ex.Message;
            return false;
        }
    }

    private void BackupCorruptFile()
    {
        try
        {
            if (!File.Exists(_paths.SettingsFile))
            {
                return;
            }

            var backup = Path.Combine(
                _paths.DataRoot,
                $"settings.invalid-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            File.Copy(_paths.SettingsFile, backup, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // 备份失败不应阻断启动,使用默认设置继续。
        }
    }
}

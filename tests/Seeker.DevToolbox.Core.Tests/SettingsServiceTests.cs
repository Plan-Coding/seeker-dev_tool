using Seeker.DevToolbox.Core.Configuration;

namespace Seeker.DevToolbox.Core.Tests;

public sealed class SettingsServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(),
        $"seeker-settings-tests-{Guid.NewGuid():N}");

    public SettingsServiceTests()
    {
        Directory.CreateDirectory(_root);
    }

    [Fact]
    public void TrySave_ThenReload_RoundTripsValues()
    {
        var paths = AppPaths.Resolve(_root, Path.Combine(_root, "fallback"));
        var service = new SettingsService(paths);

        service.Current.Theme = "Dark";
        service.Current.LastToolId = "json";
        service.Current.RecentToolIds.Add("json");
        service.Current.WindowWidth = 1280;

        Assert.True(service.TrySave(out var error));
        Assert.Null(error);

        var reloaded = new SettingsService(paths);

        Assert.Equal("Dark", reloaded.Current.Theme);
        Assert.Equal("json", reloaded.Current.LastToolId);
        Assert.Single(reloaded.Current.RecentToolIds);
        Assert.Equal(1280, reloaded.Current.WindowWidth);
    }

    [Fact]
    public void Load_CorruptFile_ReturnsDefaultsAndCreatesBackup()
    {
        var paths = AppPaths.Resolve(_root, Path.Combine(_root, "fallback"));
        Directory.CreateDirectory(paths.DataRoot);
        File.WriteAllText(paths.SettingsFile, "{ not valid json");

        var service = new SettingsService(paths);

        Assert.Equal("System", service.Current.Theme);
        Assert.NotEmpty(Directory.GetFiles(paths.DataRoot, "settings.invalid-*.json"));
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
            // 测试清理失败不影响结果。
        }
    }
}

using Seeker.DevToolbox.Core.Configuration;

namespace Seeker.DevToolbox.Core.Tests;

public sealed class AppPathsTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(),
        $"seeker-devtoolbox-tests-{Guid.NewGuid():N}");

    public AppPathsTests()
    {
        Directory.CreateDirectory(_root);
    }

    [Fact]
    public void Resolve_WithoutPortableMarker_UsesFallbackRoot()
    {
        var exeDir = Path.Combine(_root, "app");
        var fallback = Path.Combine(_root, "local");
        Directory.CreateDirectory(exeDir);

        var paths = AppPaths.Resolve(exeDir, fallback);

        Assert.False(paths.IsPortable);
        Assert.Equal(fallback, paths.DataRoot);
        Assert.True(Directory.Exists(paths.LogsDirectory));
    }

    [Fact]
    public void Resolve_WithPortableMarker_UsesDataFolderNextToExe()
    {
        var exeDir = Path.Combine(_root, "app");
        var fallback = Path.Combine(_root, "local");
        Directory.CreateDirectory(exeDir);
        File.WriteAllText(Path.Combine(exeDir, "portable.marker"), string.Empty);

        var paths = AppPaths.Resolve(exeDir, fallback);

        Assert.True(paths.IsPortable);
        Assert.Equal(Path.Combine(exeDir, "data"), paths.DataRoot);
        Assert.Equal(Path.Combine(paths.DataRoot, "settings.json"), paths.SettingsFile);
        Assert.True(Directory.Exists(paths.LogsDirectory));
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

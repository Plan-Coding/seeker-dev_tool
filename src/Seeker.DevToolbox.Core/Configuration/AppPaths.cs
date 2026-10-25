namespace Seeker.DevToolbox.Core.Configuration;

/// <summary>
/// 应用的数据目录布局。便携模式下所有数据都留在程序目录,不写注册表。
/// </summary>
public sealed record AppPaths(
    string DataRoot,
    string SettingsFile,
    string LogsDirectory,
    bool IsPortable)
{
    private const string PortableMarkerFileName = "portable.marker";

    public static AppPaths Resolve()
    {
        var exeDirectory = AppContext.BaseDirectory;
        var fallbackRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Seeker",
            "DevToolbox");

        return Resolve(exeDirectory, fallbackRoot);
    }

    /// <summary>
    /// 可测试的重载。存在 portable.marker 且目录可写时使用程序目录,否则回退到用户目录。
    /// </summary>
    public static AppPaths Resolve(string exeDirectory, string fallbackRoot)
    {
        var portable = File.Exists(Path.Combine(exeDirectory, PortableMarkerFileName));
        var dataRoot = portable && CanWrite(Path.Combine(exeDirectory, "data"))
            ? Path.Combine(exeDirectory, "data")
            : fallbackRoot;

        var logs = Path.Combine(dataRoot, "logs");
        Directory.CreateDirectory(dataRoot);
        Directory.CreateDirectory(logs);

        return new AppPaths(
            DataRoot: dataRoot,
            SettingsFile: Path.Combine(dataRoot, "settings.json"),
            LogsDirectory: logs,
            IsPortable: portable && dataRoot.StartsWith(exeDirectory, StringComparison.OrdinalIgnoreCase));
    }

    private static bool CanWrite(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
            var probe = Path.Combine(directory, $".write-probe-{Guid.NewGuid():N}");
            using (File.Create(probe, 1, FileOptions.DeleteOnClose))
            {
                // 打开即验证,关闭时自动删除
            }

            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}

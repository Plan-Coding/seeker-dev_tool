using Seeker.DevToolbox.Core.Configuration;

namespace Seeker.DevToolbox.Tools.Welcome;

public sealed class WelcomeViewModel
{
    public WelcomeViewModel(AppPaths paths, string version, int toolCount)
    {
        DataRoot = paths.DataRoot;
        IsPortable = paths.IsPortable;
        Mode = paths.IsPortable ? "便携模式" : "标准模式";
        ModeHint = paths.IsPortable
            ? "数据保存在程序目录,复制文件夹即可迁移"
            : "数据保存在用户目录,程序目录可只读";
        Version = version;
        ToolCount = toolCount;
    }

    public string DataRoot { get; }

    public bool IsPortable { get; }

    public string Mode { get; }

    public string ModeHint { get; }

    public string Version { get; }

    public int ToolCount { get; }
}

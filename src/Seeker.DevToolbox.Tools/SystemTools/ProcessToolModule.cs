using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.SystemTools;

public sealed class ProcessToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "processes",
        Title: "进程查看",
        Category: ToolCategories.System,
        Glyph: "TaskList24",
        Description: "查看进程 PID、内存占用、启动时间与可执行文件路径",
        Keywords: ["process", "pid", "memory", "task", "进程", "内存", "任务"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new ProcessToolView();
}

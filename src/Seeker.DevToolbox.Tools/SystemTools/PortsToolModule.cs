using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.SystemTools;

public sealed class PortsToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "ports",
        Title: "端口占用",
        Category: ToolCategories.System,
        Glyph: "SerialPort24",
        Description: "查看本机 TCP 监听端口、PID 与对应进程",
        Keywords: ["port", "netstat", "pid", "listen", "端口", "占用", "进程"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new PortsToolView();
}

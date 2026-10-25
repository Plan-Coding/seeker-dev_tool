using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Time;

public sealed class UnixTimeToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "timestamp",
        Title: "时间戳转换",
        Category: ToolCategories.Time,
        Glyph: "Timer24",
        Description: "Unix 秒/毫秒时间戳与本地时间、UTC、ISO 8601 互转",
        Keywords: ["timestamp", "unix", "time", "date", "时间戳", "日期", "时间", "iso8601"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new UnixTimeToolView();
}

using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Text;

public sealed class RegexToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "regex",
        Title: "正则测试",
        Category: ToolCategories.Text,
        Glyph: "Search24",
        Description: "匹配、分组查看与替换预览,支持忽略大小写/多行/单行",
        Keywords: ["regex", "regexp", "正则", "匹配", "替换", "分组"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new RegexToolView();
}

using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Text;

public sealed class TextToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "text",
        Title: "文本处理",
        Category: ToolCategories.Text,
        Glyph: "TextFont24",
        Description: "命名风格转换、大小写、排序、去重、去空行、加行号",
        Keywords: ["text", "case", "camel", "snake", "kebab", "sort", "dedupe", "文本", "驼峰", "下划线", "排序", "去重"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new TextToolView();
}

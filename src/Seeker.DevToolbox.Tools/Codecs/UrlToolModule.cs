using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Codecs;

public sealed class UrlToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "url",
        Title: "URL 编解码",
        Category: ToolCategories.Encoding,
        Glyph: "Link24",
        Description: "按 URL 组件规则编码/解码,支持表单 + 空格语义",
        Keywords: ["url", "uri", "encode", "decode", "urlencode", "编码", "解码"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new UrlToolView();
}

using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Codecs;

public sealed class Base64ToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "base64",
        Title: "Base64 编解码",
        Category: ToolCategories.Encoding,
        Glyph: "CodeBlock24",
        Description: "文本与 Base64 互转,支持 URL-safe 变体",
        Keywords: ["base64", "encode", "decode", "编码", "解码", "urlsafe"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new Base64ToolView();
}

using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.TextEncodings;

public sealed class EncodingToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "encoding",
        Title: "编码转换",
        Category: ToolCategories.Encoding,
        Glyph: "TextFont24",
        Description: "GBK/UTF-8/Big5/UTF-16 互转,用于修复乱码",
        Keywords: ["encoding", "gbk", "utf8", "big5", "mojibake", "乱码", "编码", "转码"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new EncodingToolView();
}


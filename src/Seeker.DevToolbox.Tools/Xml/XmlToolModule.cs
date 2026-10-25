using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Xml;

public sealed class XmlToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "xml",
        Title: "XML 格式化",
        Category: ToolCategories.Data,
        Glyph: "Code24",
        Description: "格式化、压缩并校验 XML,保留声明与中文内容",
        Keywords: ["xml", "format", "minify", "格式化", "压缩", "校验"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new XmlToolView();
}

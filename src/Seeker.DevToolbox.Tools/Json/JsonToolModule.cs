using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Json;

public sealed class JsonToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "json",
        Title: "JSON 格式化",
        Category: ToolCategories.Data,
        Glyph: "Braces24",
        Description: "格式化、压缩并校验 JSON,支持尾逗号与注释",
        Keywords: ["json", "format", "beautify", "minify", "格式化", "压缩", "校验"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new JsonToolView();
}

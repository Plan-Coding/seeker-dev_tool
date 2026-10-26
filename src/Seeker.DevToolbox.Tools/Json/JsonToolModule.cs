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
        Description: "格式化、压缩并校验 JSON,支持 JSON5/注释/尾逗号",
        Keywords: ["json", "json5", "jsonc", "format", "beautify", "minify", "格式化", "压缩", "校验"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new JsonToolView();
}

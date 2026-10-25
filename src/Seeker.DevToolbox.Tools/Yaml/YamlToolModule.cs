using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Yaml;

public sealed class YamlToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "yaml",
        Title: "YAML / JSON",
        Category: ToolCategories.Data,
        Glyph: "ArrowSwap24",
        Description: "YAML ↔ JSON 互转,以及 YAML 格式化与校验",
        Keywords: ["yaml", "yml", "json", "convert", "转换", "格式化"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new YamlToolView();
}

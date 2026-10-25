using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Design;

public sealed class ColorToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "color",
        Title: "颜色转换",
        Category: ToolCategories.Design,
        Glyph: "Color24",
        Description: "HEX / RGB / HSL / HSV 互转,带颜色预览",
        Keywords: ["color", "colour", "hex", "rgb", "hsl", "hsv", "颜色", "取色"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new ColorToolView();
}

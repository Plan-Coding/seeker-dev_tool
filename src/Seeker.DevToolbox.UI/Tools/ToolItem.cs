using Wpf.Ui.Controls;
using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.UI.Tools;

/// <summary>
/// 导航项/搜索结果使用的轻量视图模型,负责把字符串字形解析成强类型图标。
/// </summary>
public sealed class ToolItem
{
    public ToolItem(IToolModule module)
    {
        Module = module;
        Symbol = Enum.TryParse<SymbolRegular>(module.Metadata.Glyph, ignoreCase: true, out var symbol)
            ? symbol
            : SymbolRegular.Wrench24;
    }

    public IToolModule Module { get; }

    public ToolMetadata Metadata => Module.Metadata;

    public string Id => Metadata.Id;

    public string Title => Metadata.Title;

    public string Category => Metadata.Category;

    public SymbolRegular Symbol { get; }

    public bool Matches(string? query) => Metadata.Matches(query);
}

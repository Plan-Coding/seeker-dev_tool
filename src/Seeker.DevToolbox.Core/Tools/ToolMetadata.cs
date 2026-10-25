namespace Seeker.DevToolbox.Core.Tools;

/// <summary>
/// 工具在导航和搜索中展示所需的元数据。保持纯数据,不依赖 WPF。
/// </summary>
public sealed record ToolMetadata(
    string Id,
    string Title,
    string Category,
    string Glyph,
    string Description,
    string[] Keywords)
{
    public bool Matches(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        var value = query.Trim();

        return Title.Contains(value, StringComparison.OrdinalIgnoreCase)
            || Category.Contains(value, StringComparison.OrdinalIgnoreCase)
            || Description.Contains(value, StringComparison.OrdinalIgnoreCase)
            || Keywords.Any(keyword => keyword.Contains(value, StringComparison.OrdinalIgnoreCase));
    }
}

public static class ToolCategories
{
    public const string Common = "常用";
    public const string Data = "数据";
    public const string Text = "文本";
    public const string Encoding = "编码";
    public const string Security = "安全";
    public const string Time = "时间";
    public const string Generate = "生成";
    public const string Design = "设计";
    public const string Network = "网络";
    public const string System = "系统";
}

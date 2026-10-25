using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Generators;

public sealed class IdToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "uuid",
        Title: "UUID 生成",
        Category: ToolCategories.Generate,
        Glyph: "Tag24",
        Description: "批量生成 UUID v4(随机)或 UUID v7(按时间有序)",
        Keywords: ["uuid", "guid", "ulid", "id", "生成", "唯一"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new IdToolView();
}

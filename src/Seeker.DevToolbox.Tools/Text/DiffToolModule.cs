using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Text;

public sealed class DiffToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "diff",
        Title: "文本 Diff",
        Category: ToolCategories.Text,
        Glyph: "Document24",
        Description: "逐行比较两段文本,高亮新增、删除与修改",
        Keywords: ["diff", "compare", "difference", "对比", "差异", "比较"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new DiffToolView();
}

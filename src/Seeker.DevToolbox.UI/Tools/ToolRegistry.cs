using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.UI.Tools;

/// <summary>
/// 工具注册表。注册顺序即导航展示顺序。
/// </summary>
public sealed class ToolRegistry
{
    public ToolRegistry(IEnumerable<IToolModule> modules)
    {
        All = modules.ToList();
    }

    public IReadOnlyList<IToolModule> All { get; }

    public IToolModule? Find(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return All.FirstOrDefault(module =>
            string.Equals(module.Metadata.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}

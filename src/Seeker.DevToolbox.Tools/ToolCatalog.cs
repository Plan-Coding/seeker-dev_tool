using Seeker.DevToolbox.Tools.Json;
using Seeker.DevToolbox.Tools.Welcome;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools;

public static class ToolCatalog
{
    /// <summary>
    /// 注册顺序即导航顺序。后续新增工具在此追加即可。
    /// </summary>
    public static IReadOnlyList<IToolModule> CreateModules() =>
    [
        new WelcomeToolModule(),
        new JsonToolModule()
    ];
}

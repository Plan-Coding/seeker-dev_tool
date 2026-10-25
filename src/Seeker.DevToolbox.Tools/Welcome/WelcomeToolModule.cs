using Microsoft.Extensions.DependencyInjection;
using Seeker.DevToolbox.Core.Configuration;
using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;
using Seeker.DevToolbox.UI.Tools;

namespace Seeker.DevToolbox.Tools.Welcome;

public sealed class WelcomeToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "welcome",
        Title: "欢迎",
        Category: ToolCategories.Common,
        Glyph: "Home24",
        Description: "工具箱首页:运行模式、数据目录与使用提示",
        Keywords: ["welcome", "home", "about", "关于", "首页", "帮助"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services)
    {
        var paths = services.GetRequiredService<AppPaths>();
        var registry = services.GetRequiredService<ToolRegistry>();
        var version = typeof(WelcomeToolModule).Assembly.GetName().Version?.ToString(3) ?? "0.1.0";

        return new WelcomeView(new WelcomeViewModel(paths, version, registry.All.Count));
    }
}

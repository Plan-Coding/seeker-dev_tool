using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Network;

public sealed class HttpToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "http",
        Title: "HTTP 请求",
        Category: ToolCategories.Network,
        Glyph: "NetworkCheck24",
        Description: "发送 GET/POST 等请求,查看状态码、响应头与响应体",
        Keywords: ["http", "https", "request", "api", "rest", "请求", "接口"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new HttpToolView();
}

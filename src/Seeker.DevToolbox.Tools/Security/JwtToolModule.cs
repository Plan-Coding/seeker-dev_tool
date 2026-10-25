using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Security;

public sealed class JwtToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "jwt",
        Title: "JWT 解析",
        Category: ToolCategories.Security,
        Glyph: "Shield24",
        Description: "解码 Header/Payload 并检查 exp/nbf(不验证签名)",
        Keywords: ["jwt", "token", "bearer", "jws", "解析", "令牌"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new JwtToolView();
}

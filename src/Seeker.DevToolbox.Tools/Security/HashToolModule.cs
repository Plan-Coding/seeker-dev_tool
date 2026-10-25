using Seeker.DevToolbox.Core.Tools;
using Seeker.DevToolbox.UI.Contracts;

namespace Seeker.DevToolbox.Tools.Security;

public sealed class HashToolModule : IToolModule
{
    public ToolMetadata Metadata { get; } = new(
        Id: "hash",
        Title: "哈希计算",
        Category: ToolCategories.Security,
        Glyph: "LockClosed24",
        Description: "MD5 / SHA-1 / SHA-256 / SHA-512,可选 HMAC 密钥",
        Keywords: ["hash", "md5", "sha1", "sha256", "sha512", "hmac", "摘要", "哈希"]);

    public System.Windows.FrameworkElement CreateView(IServiceProvider services) => new HashToolView();
}

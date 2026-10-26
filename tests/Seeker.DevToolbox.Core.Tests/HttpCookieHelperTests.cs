using Seeker.DevToolbox.Core.Tools.Network;

namespace Seeker.DevToolbox.Core.Tests;

public class HttpCookieHelperTests
{
    [Fact]
    public void ExtractSetCookies_ReturnsNameValuePairs()
    {
        const string headers = """
            Content-Type: application/json
            Set-Cookie: session=abc123; Path=/; HttpOnly
            set-cookie: theme=dark; Path=/
            """;

        var cookies = HttpCookieHelper.ExtractSetCookies(headers);

        Assert.Equal(["session=abc123", "theme=dark"], cookies);
    }

    [Fact]
    public void MergeCookieHeader_ReplacesExistingValues()
    {
        var result = HttpCookieHelper.MergeCookieHeader(
            "session=old; lang=zh",
            ["session=new", "theme=dark"]);

        Assert.Equal("session=new; lang=zh; theme=dark", result);
    }

    [Fact]
    public void ExtractSetCookies_IgnoresInvalidLines()
    {
        Assert.Empty(HttpCookieHelper.ExtractSetCookies("Set-Cookie: invalid\r\nContent-Type: text/plain"));
    }
}

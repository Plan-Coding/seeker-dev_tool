using System.Text;
using Seeker.DevToolbox.Core.Tools.Security;

namespace Seeker.DevToolbox.Core.Tests;

public class JwtParserTests
{
    private static string Base64Url(string text) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    [Fact]
    public void Parse_ValidToken_ReturnsHeaderPayloadAndClaims()
    {
        var header = Base64Url("""{"alg":"HS256","typ":"JWT"}""");
        var payload = Base64Url("""{"sub":"123","exp":1700000000,"iat":1690000000}""");
        var token = $"{header}.{payload}.signature";

        Assert.True(JwtParser.TryParse(token, out var info, out var error));

        Assert.Null(error);
        Assert.NotNull(info);
        Assert.Equal("HS256", info.Algorithm);
        Assert.Equal("JWT", info.Type);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), info.ExpiresAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690000000), info.IssuedAt);
        Assert.Null(info.NotBefore);
        Assert.True(info.HasSignature);
        Assert.Contains("\"alg\": \"HS256\"", info.HeaderJson);
        Assert.Contains("\"sub\": \"123\"", info.PayloadJson);
    }

    [Fact]
    public void Parse_AcceptsBearerPrefix()
    {
        var token = $"{Base64Url("""{"alg":"none"}""")}.{Base64Url("""{"sub":"1"}""")}.sig";

        Assert.True(JwtParser.TryParse($"Bearer {token}", out var info, out _));
        Assert.Equal("none", info!.Algorithm);
    }

    [Theory]
    [InlineData("only.two")]
    [InlineData("not-a-jwt")]
    [InlineData("")]
    public void Parse_MalformedToken_ReturnsError(string token)
    {
        Assert.False(JwtParser.TryParse(token, out _, out var error));
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_InvalidJsonPayload_ReturnsError()
    {
        var token = $"{Base64Url("""{"alg":"HS256"}""")}.{Base64Url("not json")}.sig";

        Assert.False(JwtParser.TryParse(token, out _, out var error));
        Assert.Contains("JWT 解析失败", error);
    }
}

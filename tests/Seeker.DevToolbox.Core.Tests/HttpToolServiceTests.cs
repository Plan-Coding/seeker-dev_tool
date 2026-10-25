using Seeker.DevToolbox.Core.Tools.Network;

namespace Seeker.DevToolbox.Core.Tests;

public class HttpToolServiceTests
{
    [Fact]
    public void ParseHeaders_TrimsAndSkipsComments()
    {
        var headers = HttpToolService.ParseHeaders("""
            # comment
            Accept: application/json
              X-Token :  abc  
            """);

        Assert.Equal(2, headers.Count);
        Assert.Equal("Accept", headers[0].Key);
        Assert.Equal("application/json", headers[0].Value);
        Assert.Equal("X-Token", headers[1].Key);
        Assert.Equal("abc", headers[1].Value);
    }

    [Theory]
    [InlineData("NoColon")]
    [InlineData(": value")]
    public void ParseHeaders_InvalidLine_Throws(string text)
    {
        Assert.Throws<FormatException>(() => HttpToolService.ParseHeaders(text));
    }

    [Fact]
    public async Task SendAsync_InvalidUrl_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            HttpToolService.SendAsync(new HttpToolRequest("GET", "not-a-url")));
    }
}


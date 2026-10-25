using System.Text.RegularExpressions;
using Seeker.DevToolbox.Core.Tools.Text;

namespace Seeker.DevToolbox.Core.Tests;

public class RegexToolTests
{
    [Fact]
    public void Matches_ReturnsIndexesAndGroups()
    {
        var ok = RegexTool.TryMatches(
            @"(\w+)@(\w+)",
            "a@b c@d",
            RegexOptions.None,
            maxMatches: 10,
            out var matches,
            out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(2, matches.Count);
        Assert.Equal("a@b", matches[0].Value);
        Assert.Equal(0, matches[0].Index);
        Assert.Equal("a", matches[0].Groups[1].Value);
        Assert.Equal("b", matches[0].Groups[2].Value);
        Assert.Equal(4, matches[1].Index);
    }

    [Fact]
    public void Matches_RespectsMaxMatches()
    {
        Assert.True(RegexTool.TryMatches(
            @"\d+",
            "1 2 3 4 5",
            RegexOptions.None,
            maxMatches: 2,
            out var matches,
            out _));

        Assert.Equal(2, matches.Count);
    }

    [Fact]
    public void Matches_InvalidPattern_ReturnsError()
    {
        Assert.False(RegexTool.TryMatches("(", "text", RegexOptions.None, 10, out _, out var error));
        Assert.Contains("正则表达式无效", error);
    }

    [Fact]
    public void Replace_ExpandsGroups()
    {
        Assert.True(RegexTool.TryReplace(
            @"(\w+)@(\w+)",
            "a@b c@d",
            "$1 at $2",
            RegexOptions.None,
            out var result,
            out var error));

        Assert.Null(error);
        Assert.Equal("a at b c at d", result);
    }

    [Fact]
    public void Matches_IgnoreCaseOption()
    {
        Assert.True(RegexTool.TryMatches(
            @"abc",
            "ABC abc",
            RegexOptions.IgnoreCase,
            10,
            out var matches,
            out _));

        Assert.Equal(2, matches.Count);
    }
}

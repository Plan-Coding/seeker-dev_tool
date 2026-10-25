using Seeker.DevToolbox.Core.Tools.Json;

namespace Seeker.DevToolbox.Core.Tests;

public class JsonFormatterTests
{
    [Fact]
    public void Format_ProducesIndentedOutput()
    {
        var result = JsonFormatter.Format("""{"a":1,"b":[1,2]}""");

        Assert.Contains("\n", result);
        Assert.Contains("\"a\": 1", result);
        Assert.Contains("\"b\": [", result);
    }

    [Fact]
    public void Format_KeepsChineseCharactersUnescaped()
    {
        var result = JsonFormatter.Format("""{"name":"工具箱"}""");

        Assert.Contains("工具箱", result);
        Assert.DoesNotContain("\\u", result);
    }

    [Fact]
    public void Format_None_ProducesMinifiedJson()
    {
        var result = JsonFormatter.Format("""{ "a" : 1, "b" : [ 1, 2 ] }""", JsonIndentStyle.None);

        Assert.Equal("""{"a":1,"b":[1,2]}""", result);
    }

    [Fact]
    public void Format_AllowsTrailingCommasAndComments()
    {
        var result = JsonFormatter.Format("""{ "a": 1, /* note */ "b": 2, }""");

        Assert.Contains("\"a\": 1", result);
        Assert.Contains("\"b\": 2", result);
    }

    [Fact]
    public void TryFormat_InvalidJson_ReturnsFriendlyError()
    {
        var ok = JsonFormatter.TryFormat(
            """{"a": }""",
            JsonIndentStyle.TwoSpaces,
            out var result,
            out var error);

        Assert.False(ok);
        Assert.Empty(result);
        Assert.NotNull(error);
        Assert.Contains("第 1 行", error);
    }

    [Theory]
    [InlineData("""{"a":1}""")]
    [InlineData("""[1,2,3]""")]
    [InlineData("\"text\"")]
    [InlineData("42")]
    [InlineData("true")]
    [InlineData("null")]
    public void TryValidate_AcceptsEveryJsonRootKind(string json)
    {
        Assert.True(JsonFormatter.TryValidate(json, out var error));
        Assert.Null(error);
    }

    [Fact]
    public void TryValidate_EmptyInput_ReturnsError()
    {
        Assert.False(JsonFormatter.TryValidate("", out var error));
        Assert.NotNull(error);
    }
}

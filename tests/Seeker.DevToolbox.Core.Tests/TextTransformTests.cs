using Seeker.DevToolbox.Core.Tools.Text;

namespace Seeker.DevToolbox.Core.Tests;

public class TextTransformTests
{
    [Fact]
    public void SplitWords_HandlesCamelAcronymsAndSeparators()
    {
        Assert.Equal(
            ["HTTP", "Server", "user", "ID", "2"],
            TextTransform.SplitWords("HTTPServer_userID2"));
    }

    [Theory]
    [InlineData("user_name", "userName")]
    [InlineData("User name", "userName")]
    [InlineData("HTTPServer", "httpServer")]
    public void ToCamelCase(string input, string expected)
    {
        Assert.Equal(expected, TextTransform.ToCamelCase(input));
    }

    [Theory]
    [InlineData("user_name", "UserName")]
    [InlineData("http-server", "HttpServer")]
    public void ToPascalCase(string input, string expected)
    {
        Assert.Equal(expected, TextTransform.ToPascalCase(input));
    }

    [Theory]
    [InlineData("HTTPServer", "http_server")]
    [InlineData("userId", "user_id")]
    public void ToSnakeCase(string input, string expected)
    {
        Assert.Equal(expected, TextTransform.ToSnakeCase(input));
    }

    [Fact]
    public void ToKebabAndConstantCase()
    {
        Assert.Equal("http-server", TextTransform.ToKebabCase("HTTPServer"));
        Assert.Equal("HTTP_SERVER", TextTransform.ToConstantCase("HTTPServer"));
        Assert.Equal("Http Server", TextTransform.ToTitleCase("HTTPServer"));
    }

    [Fact]
    public void SortLines_RespectsDirectionAndCase()
    {
        Assert.Equal("a\nB\nc", TextTransform.SortLines("c\na\nB", ignoreCase: true));
        Assert.Equal("c\nB\na", TextTransform.SortLines("c\na\nB", descending: true, ignoreCase: true));
    }

    [Fact]
    public void RemoveDuplicateLines_IgnoreCase()
    {
        Assert.Equal("a\nB\nc", TextTransform.RemoveDuplicateLines("a\nB\nA\nc", ignoreCase: true));
    }

    [Fact]
    public void RemoveEmptyLines_DropsWhitespaceOnlyLines()
    {
        Assert.Equal("a\nb", TextTransform.RemoveEmptyLines("a\n\n  \nb"));
    }

    [Fact]
    public void AddLineNumbers_PadsWidth()
    {
        Assert.Equal(" 9. a\n10. b\n11. c", TextTransform.AddLineNumbers("a\nb\nc", start: 9));
    }

    [Fact]
    public void TrimLines_RemovesSurroundingWhitespace()
    {
        Assert.Equal("a\nb", TextTransform.TrimLines("  a  \n\tb "));
    }
}

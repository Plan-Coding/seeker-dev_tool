using Seeker.DevToolbox.Core.Tools;

namespace Seeker.DevToolbox.Core.Tests;

public class ToolMetadataTests
{
    private static readonly ToolMetadata Metadata = new(
        Id: "json",
        Title: "JSON 格式化",
        Category: ToolCategories.Data,
        Glyph: "Braces24",
        Description: "格式化、压缩并校验 JSON",
        Keywords: ["json", "format", "格式化", "压缩"]);

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Matches_EmptyQuery_ReturnsTrue(string? query)
    {
        Assert.True(Metadata.Matches(query));
    }

    [Theory]
    [InlineData("json")]
    [InlineData("JSON")]
    [InlineData("格式化")]
    [InlineData("压缩")]
    [InlineData("数据")]
    [InlineData("校验")]
    public void Matches_KnownTerms_ReturnsTrue(string query)
    {
        Assert.True(Metadata.Matches(query));
    }

    [Fact]
    public void Matches_UnknownTerm_ReturnsFalse()
    {
        Assert.False(Metadata.Matches("base64"));
    }
}

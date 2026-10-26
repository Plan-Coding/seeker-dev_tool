using Seeker.DevToolbox.Core.Tools.Json;

namespace Seeker.DevToolbox.Core.Tests;

public class Json5ConverterTests
{
    [Fact]
    public void ToJson_ConvertsJson5Features()
    {
        const string input = """
            {
              // JSON5 comment
              name: 'Seeker',
              version: 1.0,
              enabled: true,
              count: 0xFF,
              tools: ['json', 'diff',],
            }
            """;

        Assert.True(Json5Converter.TryToJson(input, out var json, out var error));

        Assert.Null(error);
        Assert.Contains("\"name\": \"Seeker\"", json);
        Assert.Contains("\"version\": 1", json);
        Assert.Contains("\"enabled\": true", json);
        Assert.Contains("\"count\": 255", json);
        Assert.Contains("\"tools\": [", json);
    }

    [Fact]
    public void ToJson_InvalidJson5_ReturnsLineAndColumn()
    {
        Assert.False(Json5Converter.TryToJson("{ name: }", out _, out var error));
        Assert.Contains("JSON5 语法错误", error);
        Assert.Contains("行", error);
    }

    [Fact]
    public void ToJson_SpecialNumbers_AreConvertedToStrings()
    {
        Assert.True(Json5Converter.TryToJson("{ value: Infinity }", out var json, out _));
        Assert.Contains("\"Infinity\"", json);
    }
}

using Seeker.DevToolbox.Core.Tools.Yaml;

namespace Seeker.DevToolbox.Core.Tests;

public class YamlJsonConverterTests
{
    [Fact]
    public void YamlToJson_ConvertsNestedStructures()
    {
        const string yaml = """
            name: Seeker
            portable: true
            tools:
              - json
              - diff
            meta:
              version: 0.4.0
            """;

        Assert.True(YamlJsonConverter.TryYamlToJson(yaml, out var json, out var error));

        Assert.Null(error);
        Assert.Contains("\"name\": \"Seeker\"", json);
        Assert.Contains("\"portable\": true", json);
        Assert.Contains("\"json\"", json);
        Assert.Contains("\"version\": \"0.4.0\"", json);
    }

    [Fact]
    public void JsonToYaml_ConvertsNestedStructures()
    {
        const string json = """{"name":"Seeker","items":[1,2],"nested":{"ok":true}}""";

        Assert.True(YamlJsonConverter.TryJsonToYaml(json, out var yaml, out var error));

        Assert.Null(error);
        Assert.Contains("name: Seeker", yaml);
        Assert.Contains("- 1", yaml);
        Assert.Contains("ok: true", yaml);
    }

    [Fact]
    public void JsonToYaml_KeepsChineseCharacters()
    {
        Assert.True(YamlJsonConverter.TryJsonToYaml("""{"name":"工具箱"}""", out var yaml, out _));
        Assert.Contains("工具箱", yaml);
    }

    [Fact]
    public void FormatYaml_ReparsesAndWrites()
    {
        const string yaml = "name:   Seeker\nitems:\n    -   json\n";

        Assert.True(YamlJsonConverter.TryFormatYaml(yaml, out var result, out var error));

        Assert.Null(error);
        Assert.Contains("name: Seeker", result);
        Assert.Contains("- json", result);
    }

    [Fact]
    public void YamlToJson_InvalidYaml_ReturnsError()
    {
        Assert.False(YamlJsonConverter.TryYamlToJson("a: [1, 2", out _, out var error));
        Assert.Contains("YAML 解析失败", error);
    }
}

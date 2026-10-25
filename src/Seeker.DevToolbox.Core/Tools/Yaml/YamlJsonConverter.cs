using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Seeker.DevToolbox.Core.Tools.Yaml;

public static class YamlJsonConverter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public static bool TryYamlToJson(string yaml, out string json, out string? error)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithAttemptingUnquotedStringTypeDeserialization()
                .Build();
            var value = deserializer.Deserialize<object?>(yaml);
            json = JsonSerializer.Serialize(Normalize(value), JsonOptions);
            error = null;
            return true;
        }
        catch (YamlException ex)
        {
            json = string.Empty;
            error = $"YAML 解析失败:第 {ex.Start.Line} 行,第 {ex.Start.Column} 列。{ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            json = string.Empty;
            error = $"YAML 转换失败:{ex.Message}";
            return false;
        }
    }

    public static bool TryJsonToYaml(string json, out string yaml, out string? error)
    {
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            var serializer = new SerializerBuilder()
                .WithIndentedSequences()
                .Build();

            yaml = serializer.Serialize(ToObject(document.RootElement));
            error = null;
            return true;
        }
        catch (JsonException ex)
        {
            yaml = string.Empty;
            var line = (ex.LineNumber ?? 0) + 1;
            var column = (ex.BytePositionInLine ?? 0) + 1;
            error = $"JSON 语法错误:第 {line} 行,第 {column} 列。{ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            yaml = string.Empty;
            error = $"JSON 转换失败:{ex.Message}";
            return false;
        }
    }

    public static bool TryFormatYaml(string yaml, out string result, out string? error)
    {
        try
        {
            var stream = new YamlStream();
            using var reader = new StringReader(yaml);
            stream.Load(reader);

            using var writer = new StringWriter();
            stream.Save(writer, assignAnchors: false);
            result = writer.ToString();
            error = null;
            return true;
        }
        catch (YamlException ex)
        {
            result = string.Empty;
            error = $"YAML 解析失败:第 {ex.Start.Line} 行,第 {ex.Start.Column} 列。{ex.Message}";
            return false;
        }
    }

    private static object? Normalize(object? value) => value switch
    {
        null => null,
        IDictionary<object, object> map => map.ToDictionary(
            pair => pair.Key?.ToString() ?? string.Empty,
            pair => Normalize(pair.Value)),
        IDictionary<string, object> map => map.ToDictionary(
            pair => pair.Key,
            pair => Normalize(pair.Value)),
        IEnumerable<object> list => list.Select(Normalize).ToList(),
        _ => value
    };

    private static object? ToObject(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Object => element.EnumerateObject()
            .ToDictionary(property => property.Name, property => ToObject(property.Value)),
        JsonValueKind.Array => element.EnumerateArray().Select(ToObject).ToList(),
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => element.TryGetInt64(out var integer)
            ? integer
            : element.TryGetDouble(out var number)
                ? number
                : element.GetRawText(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => null
    };
}

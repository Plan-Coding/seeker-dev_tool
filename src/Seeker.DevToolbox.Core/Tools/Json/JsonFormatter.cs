using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Seeker.DevToolbox.Core.Tools.Json;

public enum JsonIndentStyle
{
    TwoSpaces,
    FourSpaces,
    Tab,
    None
}

/// <summary>
/// 纯 JSON 格式化逻辑,不依赖 UI,方便单测和后续复用(CLI/插件)。
/// </summary>
public static class JsonFormatter
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    public static string Format(string input, JsonIndentStyle style = JsonIndentStyle.TwoSpaces)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var document = JsonDocument.Parse(input, DocumentOptions);

        var writerOptions = new JsonWriterOptions
        {
            Indented = style != JsonIndentStyle.None,
            IndentCharacter = style == JsonIndentStyle.Tab ? '\t' : ' ',
            IndentSize = style switch
            {
                JsonIndentStyle.FourSpaces => 4,
                JsonIndentStyle.Tab => 1,
                _ => 2
            },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, writerOptions))
        {
            document.RootElement.WriteTo(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static bool TryFormat(
        string input,
        JsonIndentStyle style,
        out string result,
        out string? error)
    {
        try
        {
            result = Format(input, style);
            error = null;
            return true;
        }
        catch (JsonException ex)
        {
            result = string.Empty;
            var line = (ex.LineNumber ?? 0) + 1;
            var column = (ex.BytePositionInLine ?? 0) + 1;
            error = $"JSON 语法错误:第 {line} 行,第 {column} 列。{ex.Message}";
            return false;
        }
    }

    public static bool TryValidate(string input, out string? error)
    {
        try
        {
            using var document = JsonDocument.Parse(input, DocumentOptions);
            _ = document.RootElement.ValueKind;
            error = null;
            return true;
        }
        catch (JsonException ex)
        {
            var line = (ex.LineNumber ?? 0) + 1;
            var column = (ex.BytePositionInLine ?? 0) + 1;
            error = $"JSON 语法错误:第 {line} 行,第 {column} 列。{ex.Message}";
            return false;
        }
    }
}

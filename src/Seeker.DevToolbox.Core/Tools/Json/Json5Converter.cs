using System.Text.Encodings.Web;
using System.Text.Json;
using Json5Net = Json5.Json5;

namespace Seeker.DevToolbox.Core.Tools.Json;

public static class Json5Converter
{
    public static bool TryToJson(string input, out string json, out string? error)
    {
        try
        {
            var readerOptions = new Json5.Json5ReaderOptions
            {
                SpecialNumbers = Json5.SpecialNumberHandling.AsString,
                MaxDepth = 128
            };

            var writerOptions = new JsonWriterOptions
            {
                Indented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            json = Json5Net.ToJson(input, readerOptions, writerOptions);
            error = null;
            return true;
        }
        catch (Json5.Json5Exception ex)
        {
            json = string.Empty;
            error = $"JSON5 语法错误:第 {ex.Line} 行,第 {ex.Column} 列。{ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            json = string.Empty;
            error = $"JSON5 转换失败:{ex.Message}";
            return false;
        }
    }
}

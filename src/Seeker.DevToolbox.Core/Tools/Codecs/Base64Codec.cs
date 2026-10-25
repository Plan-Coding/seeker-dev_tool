using System.Text;

namespace Seeker.DevToolbox.Core.Tools.Codecs;

public static class Base64Codec
{
    public static string Encode(string text, bool urlSafe = false)
    {
        ArgumentNullException.ThrowIfNull(text);

        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

        return urlSafe
            ? base64.TrimEnd('=').Replace('+', '-').Replace('/', '_')
            : base64;
    }

    public static bool TryDecode(string text, out string result, out string? error)
    {
        try
        {
            var normalized = Normalize(text);
            var bytes = Convert.FromBase64String(normalized);
            result = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes);
            error = null;
            return true;
        }
        catch (FormatException)
        {
            result = string.Empty;
            error = "不是有效的 Base64 文本";
            return false;
        }
        catch (DecoderFallbackException)
        {
            result = string.Empty;
            error = "Base64 解出的字节不是有效的 UTF-8 文本";
            return false;
        }
    }

    private static string Normalize(string text)
    {
        var value = text
            .Replace("-", "+")
            .Replace("_", "/")
            .Replace(" ", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty)
            .Trim();

        return (value.Length % 4) switch
        {
            0 => value,
            2 => value + "==",
            3 => value + "=",
            _ => throw new FormatException("Invalid Base64 length.")
        };
    }
}

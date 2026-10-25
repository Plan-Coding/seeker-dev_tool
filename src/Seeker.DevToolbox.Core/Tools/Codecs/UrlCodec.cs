namespace Seeker.DevToolbox.Core.Tools.Codecs;

public static class UrlCodec
{
    /// <summary>
    /// 按 URL 组件规则编码(等价于 JavaScript 的 encodeURIComponent)。
    /// </summary>
    public static string Encode(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return Uri.EscapeDataString(text);
    }

    /// <summary>
    /// 解码 URL 组件。plusAsSpace 对应表单编码(x-www-form-urlencoded)里 + 表示空格。
    /// </summary>
    public static bool TryDecode(string text, bool plusAsSpace, out string result, out string? error)
    {
        var value = plusAsSpace ? text.Replace("+", " ") : text;

        if (!HasValidEscapes(value))
        {
            result = string.Empty;
            error = "包含无效的百分号转义序列,例如 %ZZ 或结尾多余的 %";
            return false;
        }

        try
        {
            result = Uri.UnescapeDataString(value);
            error = null;
            return true;
        }
        catch (Exception ex) when (ex is UriFormatException or ArgumentException)
        {
            result = string.Empty;
            error = $"URL 解码失败:{ex.Message}";
            return false;
        }
    }

    private static bool HasValidEscapes(string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != '%')
            {
                continue;
            }

            if (i + 2 >= text.Length || !IsHex(text[i + 1]) || !IsHex(text[i + 2]))
            {
                return false;
            }

            i += 2;
        }

        return true;
    }

    private static bool IsHex(char value) =>
        value is >= '0' and <= '9'
        or >= 'a' and <= 'f'
        or >= 'A' and <= 'F';
}

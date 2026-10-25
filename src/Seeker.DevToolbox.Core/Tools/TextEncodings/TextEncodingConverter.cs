using System.Text;

namespace Seeker.DevToolbox.Core.Tools.TextEncodings;

public sealed record EncodingOption(string Id, string DisplayName);

public static class TextEncodingConverter
{
    static TextEncodingConverter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static IReadOnlyList<EncodingOption> Options { get; } =
    [
        new("utf-8", "UTF-8"),
        new("gbk", "GBK / GB2312"),
        new("gb18030", "GB18030"),
        new("big5", "Big5"),
        new("utf-16-le", "UTF-16 LE"),
        new("utf-16-be", "UTF-16 BE"),
        new("latin1", "ISO-8859-1")
    ];

    public static bool TryConvert(
        string text,
        string sourceId,
        string targetId,
        out string result,
        out string? error)
    {
        try
        {
            var source = GetEncoding(sourceId);
            var target = GetEncoding(targetId);
            var bytes = source.GetBytes(text);
            result = target.GetString(bytes);
            error = null;
            return true;
        }
        catch (EncoderFallbackException)
        {
            result = string.Empty;
            error = "源编码无法表示输入文本,请换一个源编码";
            return false;
        }
        catch (DecoderFallbackException)
        {
            result = string.Empty;
            error = "转换出的字节在目标编码中无效,请换一个目标编码";
            return false;
        }
        catch (ArgumentException ex)
        {
            result = string.Empty;
            error = $"编码不受支持:{ex.Message}";
            return false;
        }
    }

    public static Encoding GetEncoding(string id) => id switch
    {
        "utf-8" => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
        "utf-16-le" => new UnicodeEncoding(bigEndian: false, byteOrderMark: false, throwOnInvalidBytes: true),
        "utf-16-be" => new UnicodeEncoding(bigEndian: true, byteOrderMark: false, throwOnInvalidBytes: true),
        "gbk" => Encoding.GetEncoding("gbk", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback),
        "gb18030" => Encoding.GetEncoding("gb18030", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback),
        "big5" => Encoding.GetEncoding("big5", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback),
        "latin1" => Encoding.GetEncoding("iso-8859-1", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback),
        _ => throw new ArgumentException($"未知编码:{id}", nameof(id))
    };
}

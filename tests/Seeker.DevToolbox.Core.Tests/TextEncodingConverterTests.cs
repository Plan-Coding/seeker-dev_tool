using System.Text;
using Seeker.DevToolbox.Core.Tools.TextEncodings;

namespace Seeker.DevToolbox.Core.Tests;

public class TextEncodingConverterTests
{
    [Fact]
    public void Convert_GbkMojibakeToUtf8_RestoresChinese()
    {
        var utf8Bytes = Encoding.UTF8.GetBytes("你好");
        var gbk = TextEncodingConverter.GetEncoding("gbk");
        var mojibake = gbk.GetString(utf8Bytes);

        Assert.True(TextEncodingConverter.TryConvert(mojibake, "gbk", "utf-8", out var result, out var error));

        Assert.Null(error);
        Assert.Equal("你好", result);
    }

    [Fact]
    public void Convert_Utf16RoundTrip()
    {
        Assert.True(TextEncodingConverter.TryConvert(
            "Seeker 工具箱",
            "utf-16-le",
            "utf-16-be",
            out var result,
            out _));

        Assert.NotEqual("Seeker 工具箱", result);
    }

    [Fact]
    public void Convert_UnsupportedEncoding_ReturnsError()
    {
        Assert.False(TextEncodingConverter.TryConvert("abc", "unknown", "utf-8", out _, out var error));
        Assert.NotNull(error);
    }

    [Fact]
    public void Convert_Latin1CannotEncodeChinese_ReturnsError()
    {
        Assert.False(TextEncodingConverter.TryConvert("中文", "latin1", "utf-8", out _, out var error));
        Assert.Contains("源编码无法表示", error);
    }
}

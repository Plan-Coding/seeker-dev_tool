using Seeker.DevToolbox.Core.Tools.Codecs;

namespace Seeker.DevToolbox.Core.Tests;

public class CodecTests
{
    [Fact]
    public void Base64_Encode_Utf8Text()
    {
        Assert.Equal("5L2g5aW9", Base64Codec.Encode("你好"));
    }

    [Fact]
    public void Base64_RoundTripsChinese()
    {
        const string original = "Seeker 工具箱:JSON/Base64";

        Assert.True(Base64Codec.TryDecode(Base64Codec.Encode(original), out var decoded, out var error));
        Assert.Null(error);
        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Base64_UrlSafe_HasNoPaddingOrPlusSlash()
    {
        var encoded = Base64Codec.Encode(">>>???", urlSafe: true);

        Assert.DoesNotContain('=', encoded);
        Assert.DoesNotContain('+', encoded);
        Assert.DoesNotContain('/', encoded);
        Assert.True(Base64Codec.TryDecode(encoded, out var decoded, out _));
        Assert.Equal(">>>???", decoded);
    }

    [Theory]
    [InlineData("!!!!")]
    [InlineData("A")]
    public void Base64_InvalidInput_ReturnsError(string input)
    {
        Assert.False(Base64Codec.TryDecode(input, out _, out var error));
        Assert.NotNull(error);
    }

    [Fact]
    public void Base64_NonUtf8Bytes_ReturnsError()
    {
        Assert.False(Base64Codec.TryDecode("/w==", out _, out var error));
        Assert.Contains("UTF-8", error);
    }

    [Fact]
    public void Url_Encode_Component()
    {
        Assert.Equal("a%20b%26c%3D1", UrlCodec.Encode("a b&c=1"));
    }

    [Fact]
    public void Url_Decode_Component()
    {
        Assert.True(UrlCodec.TryDecode("a%20b%26c%3D1", plusAsSpace: false, out var result, out _));
        Assert.Equal("a b&c=1", result);
    }

    [Fact]
    public void Url_Decode_PlusAsSpace()
    {
        Assert.True(UrlCodec.TryDecode("hello+world", plusAsSpace: true, out var result, out _));
        Assert.Equal("hello world", result);
    }

    [Theory]
    [InlineData("%ZZ")]
    [InlineData("%2")]
    [InlineData("100%")]
    public void Url_InvalidEscapes_ReturnsError(string input)
    {
        Assert.False(UrlCodec.TryDecode(input, plusAsSpace: false, out _, out var error));
        Assert.NotNull(error);
    }
}

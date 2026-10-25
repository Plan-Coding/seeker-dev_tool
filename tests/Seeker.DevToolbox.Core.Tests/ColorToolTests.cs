using Seeker.DevToolbox.Core.Tools.Design;

namespace Seeker.DevToolbox.Core.Tests;

public class ColorToolTests
{
    [Fact]
    public void Parse_Hex_PrimaryRed()
    {
        Assert.True(ColorTool.TryParse("#FF0000", out var color, out var error));

        Assert.Null(error);
        Assert.Equal(new RgbColor(255, 0, 0), color);
        Assert.Equal("#FF0000", ColorTool.ToHex(color));
        Assert.Equal("rgb(255, 0, 0)", ColorTool.ToRgbString(color));
        Assert.Equal("hsl(0, 100%, 50%)", ColorTool.ToHslString(color));
        Assert.Equal("hsv(0, 100%, 100%)", ColorTool.ToHsvString(color));
    }

    [Fact]
    public void Parse_ShortHex_ExpandsDigits()
    {
        Assert.True(ColorTool.TryParse("#0f0", out var color, out _));
        Assert.Equal(new RgbColor(0, 255, 0), color);
    }

    [Fact]
    public void Parse_RgbFunction()
    {
        Assert.True(ColorTool.TryParse("rgb(0, 128, 255)", out var color, out _));
        Assert.Equal("#0080FF", ColorTool.ToHex(color));
    }

    [Fact]
    public void Parse_HslFunction()
    {
        Assert.True(ColorTool.TryParse("hsl(210, 100%, 50%)", out var color, out _));
        Assert.Equal(new RgbColor(0, 128, 255), color);
    }

    [Theory]
    [InlineData("#GG0000")]
    [InlineData("#12345")]
    [InlineData("rgb(300, 0, 0)")]
    [InlineData("hsl(0, 200%, 50%)")]
    [InlineData("not-a-color")]
    public void Parse_InvalidInput_ReturnsError(string input)
    {
        Assert.False(ColorTool.TryParse(input, out _, out var error));
        Assert.NotNull(error);
    }

    [Fact]
    public void FromHsl_RoundTripsHueAndLightness()
    {
        var color = ColorTool.FromHsl(120, 100, 25);
        var (h, s, l) = ColorTool.ToHsl(color);

        Assert.Equal(120, h, 0);
        Assert.Equal(100, s, 0);
        Assert.Equal(25, l, 0);
    }
}

using Seeker.DevToolbox.Core.Tools.Time;

namespace Seeker.DevToolbox.Core.Tests;

public class UnixTimeTests
{
    [Fact]
    public void Parse_Seconds()
    {
        Assert.True(UnixTime.TryParseTimestamp("1700000000", UnixUnit.Seconds, out var result, out var used, out _));

        Assert.Equal(UnixUnit.Seconds, used);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), result);
        Assert.Equal("2023-11-14T22:13:20.000Z", UnixTime.FormatIso(result));
    }

    [Fact]
    public void Parse_Milliseconds()
    {
        Assert.True(UnixTime.TryParseTimestamp("1700000000123", UnixUnit.Milliseconds, out var result, out var used, out _));

        Assert.Equal(UnixUnit.Milliseconds, used);
        Assert.Equal(1700000000123, result.ToUnixTimeMilliseconds());
    }

    [Fact]
    public void Parse_AutoDetectsMilliseconds()
    {
        Assert.True(UnixTime.TryParseTimestamp("1700000000123", UnixUnit.Auto, out _, out var used, out _));
        Assert.Equal(UnixUnit.Milliseconds, used);

        Assert.True(UnixTime.TryParseTimestamp("1700000000", UnixUnit.Auto, out _, out used, out _));
        Assert.Equal(UnixUnit.Seconds, used);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    public void Parse_InvalidTimestamp_ReturnsError(string input)
    {
        Assert.False(UnixTime.TryParseTimestamp(input, UnixUnit.Auto, out _, out _, out var error));
        Assert.NotNull(error);
    }

    [Fact]
    public void Parse_DateTime_CommonFormat()
    {
        Assert.True(UnixTime.TryParseDateTime("2026-10-25 12:30:00", out var result, out var error));

        Assert.Null(error);
        Assert.Equal(new DateTime(2026, 10, 25, 12, 30, 0), result.DateTime);
    }

    [Fact]
    public void Parse_DateTime_IsoFormat()
    {
        Assert.True(UnixTime.TryParseDateTime("2026-10-25T12:30:00Z", out var result, out _));
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1792931400), result);
    }

    [Fact]
    public void Parse_InvalidDateTime_ReturnsError()
    {
        Assert.False(UnixTime.TryParseDateTime("not a date", out _, out var error));
        Assert.NotNull(error);
    }
}

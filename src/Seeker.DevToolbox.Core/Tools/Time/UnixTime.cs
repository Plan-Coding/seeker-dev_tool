using System.Globalization;

namespace Seeker.DevToolbox.Core.Tools.Time;

public enum UnixUnit
{
    Auto,
    Seconds,
    Milliseconds
}

public static class UnixTime
{
    private const long MillisecondThreshold = 100_000_000_000L;

    private static readonly string[] DateTimeFormats =
    [
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd HH:mm",
        "yyyy/MM/dd"
    ];

    public static bool TryParseTimestamp(
        string input,
        UnixUnit unit,
        out DateTimeOffset result,
        out UnixUnit usedUnit,
        out string? error)
    {
        result = default;
        usedUnit = unit;

        if (!long.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            error = "请输入整数时间戳";
            return false;
        }

        var magnitude = value == long.MinValue ? long.MaxValue : Math.Abs(value);

        usedUnit = unit == UnixUnit.Auto
            ? (magnitude >= MillisecondThreshold ? UnixUnit.Milliseconds : UnixUnit.Seconds)
            : unit;

        try
        {
            result = usedUnit == UnixUnit.Milliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(value)
                : DateTimeOffset.FromUnixTimeSeconds(value);
            error = null;
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            error = "时间戳超出支持范围(约 1970-01-01 到 9999-12-31)";
            return false;
        }
    }

    public static bool TryParseDateTime(string input, out DateTimeOffset result, out string? error)
    {
        var value = input.Trim();

        if (DateTimeOffset.TryParseExact(
                value,
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out result))
        {
            error = null;
            return true;
        }

        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out result))
        {
            error = null;
            return true;
        }

        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out result))
        {
            error = null;
            return true;
        }

        error = "无法识别的时间格式,试试 2026-10-25 12:30:00 或 ISO 8601";
        return false;
    }

    public static string FormatLocal(DateTimeOffset value) =>
        value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);

    public static string FormatUtc(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff 'UTC'", CultureInfo.InvariantCulture);

    public static string FormatIso(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
}

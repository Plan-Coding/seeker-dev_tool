using System.Globalization;
using System.Text.RegularExpressions;

namespace Seeker.DevToolbox.Core.Tools.Design;

public readonly record struct RgbColor(byte R, byte G, byte B);

public static class ColorTool
{
    private static readonly Regex NumberRegex = new(
        @"-?\d+(?:\.\d+)?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryParse(string input, out RgbColor color, out string? error)
    {
        color = default;
        var value = input.Trim();

        if (value.Length == 0)
        {
            error = "请输入颜色值";
            return false;
        }

        if (value.StartsWith('#'))
        {
            return TryParseHex(value[1..], out color, out error);
        }

        if (value.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseRgb(value, out color, out error);
        }

        if (value.StartsWith("hsl", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHsl(value, out color, out error);
        }

        error = "无法识别,支持 #RGB、#RRGGBB、rgb(...)、hsl(...)";
        return false;
    }

    public static string ToHex(RgbColor color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    public static string ToRgbString(RgbColor color) => $"rgb({color.R}, {color.G}, {color.B})";

    public static (double H, double S, double L) ToHsl(RgbColor color)
    {
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;
        var lightness = (max + min) / 2;

        if (delta == 0)
        {
            return (0, 0, lightness * 100);
        }

        var saturation = lightness > 0.5
            ? delta / (2 - max - min)
            : delta / (max + min);

        var hue = max switch
        {
            _ when max == r => ((g - b) / delta + (g < b ? 6 : 0)) * 60,
            _ when max == g => ((b - r) / delta + 2) * 60,
            _ => ((r - g) / delta + 4) * 60
        };

        return (hue, saturation * 100, lightness * 100);
    }

    public static string ToHslString(RgbColor color)
    {
        var (h, s, l) = ToHsl(color);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"hsl({h:0.#}, {s:0.#}%, {l:0.#}%)");
    }

    public static (double H, double S, double V) ToHsv(RgbColor color)
    {
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        var hue = 0.0;

        if (delta != 0)
        {
            hue = max switch
            {
                _ when max == r => ((g - b) / delta + (g < b ? 6 : 0)) * 60,
                _ when max == g => ((b - r) / delta + 2) * 60,
                _ => ((r - g) / delta + 4) * 60
            };
        }

        var saturation = max == 0 ? 0 : delta / max;
        return (hue, saturation * 100, max * 100);
    }

    public static string ToHsvString(RgbColor color)
    {
        var (h, s, v) = ToHsv(color);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"hsv({h:0.#}, {s:0.#}%, {v:0.#}%)");
    }

    public static RgbColor FromHsl(double hue, double saturation, double lightness)
    {
        hue = ((hue % 360) + 360) % 360;
        saturation = Math.Clamp(saturation, 0, 100) / 100.0;
        lightness = Math.Clamp(lightness, 0, 100) / 100.0;

        var chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        var hPrime = hue / 60.0;
        var x = chroma * (1 - Math.Abs(hPrime % 2 - 1));
        var m = lightness - chroma / 2;

        var (r, g, b) = hPrime switch
        {
            < 1 => (chroma, x, 0.0),
            < 2 => (x, chroma, 0.0),
            < 3 => (0.0, chroma, x),
            < 4 => (0.0, x, chroma),
            < 5 => (x, 0.0, chroma),
            _ => (chroma, 0.0, x)
        };

        return new RgbColor(ToByte(r + m), ToByte(g + m), ToByte(b + m));
    }

    private static byte ToByte(double value) =>
        (byte)Math.Round(Math.Clamp(value, 0, 1) * 255, MidpointRounding.AwayFromZero);

    private static bool TryParseHex(string hex, out RgbColor color, out string? error)
    {
        color = default;

        if (hex.Length == 3)
        {
            hex = string.Concat(hex.Select(ch => $"{ch}{ch}"));
        }

        if (hex.Length != 6 || !hex.All(Uri.IsHexDigit))
        {
            error = "十六进制颜色应为 #RGB 或 #RRGGBB";
            return false;
        }

        color = new RgbColor(
            byte.Parse(hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(hex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(hex[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture));

        error = null;
        return true;
    }

    private static bool TryParseRgb(string input, out RgbColor color, out string? error)
    {
        color = default;

        var numbers = NumberRegex.Matches(input)
            .Select(match => double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : double.NaN)
            .ToList();

        if (numbers.Count < 3 || numbers.Any(double.IsNaN))
        {
            error = "rgb(...) 需要 3 个数字,例如 rgb(255, 128, 0)";
            return false;
        }

        if (numbers.Take(3).Any(value => value is < 0 or > 255))
        {
            error = "RGB 分量必须在 0–255 之间";
            return false;
        }

        color = new RgbColor((byte)numbers[0], (byte)numbers[1], (byte)numbers[2]);
        error = null;
        return true;
    }

    private static bool TryParseHsl(string input, out RgbColor color, out string? error)
    {
        color = default;

        var numbers = NumberRegex.Matches(input)
            .Select(match => double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : double.NaN)
            .ToList();

        if (numbers.Count < 3 || numbers.Any(double.IsNaN))
        {
            error = "hsl(...) 需要 3 个数字,例如 hsl(210, 100%, 50%)";
            return false;
        }

        if (numbers[1] is < 0 or > 100 || numbers[2] is < 0 or > 100)
        {
            error = "HSL 的饱和度和亮度必须在 0–100 之间";
            return false;
        }

        color = FromHsl(numbers[0], numbers[1], numbers[2]);
        error = null;
        return true;
    }
}

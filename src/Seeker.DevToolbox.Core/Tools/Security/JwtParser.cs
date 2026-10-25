using System.Text;
using System.Text.Json;
using Seeker.DevToolbox.Core.Tools.Json;

namespace Seeker.DevToolbox.Core.Tools.Security;

public sealed record JwtTokenInfo(
    string HeaderJson,
    string PayloadJson,
    string? Algorithm,
    string? Type,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? NotBefore,
    DateTimeOffset? IssuedAt,
    bool HasSignature);

public static class JwtParser
{
    public static bool TryParse(string token, out JwtTokenInfo? info, out string? error)
    {
        info = null;

        var value = token.Trim();

        if (value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            value = value[7..].Trim();
        }

        var segments = value.Split('.');

        if (segments.Length != 3)
        {
            error = "JWT 必须由 header.payload.signature 三段组成";
            return false;
        }

        try
        {
            var headerJson = DecodeJson(segments[0]);
            var payloadJson = DecodeJson(segments[1]);

            using var header = JsonDocument.Parse(headerJson);
            using var payload = JsonDocument.Parse(payloadJson);

            info = new JwtTokenInfo(
                HeaderJson: headerJson,
                PayloadJson: payloadJson,
                Algorithm: GetString(header.RootElement, "alg"),
                Type: GetString(header.RootElement, "typ"),
                ExpiresAt: GetUnixTime(payload.RootElement, "exp"),
                NotBefore: GetUnixTime(payload.RootElement, "nbf"),
                IssuedAt: GetUnixTime(payload.RootElement, "iat"),
                HasSignature: segments[2].Length > 0);

            error = null;
            return true;
        }
        catch (Exception ex) when (ex is FormatException or JsonException or DecoderFallbackException)
        {
            error = $"JWT 解析失败:{ex.Message}";
            return false;
        }
    }

    private static string DecodeJson(string segment)
    {
        var bytes = DecodeBase64Url(segment);
        var json = new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes);
        return JsonFormatter.Format(json);
    }

    private static byte[] DecodeBase64Url(string segment)
    {
        var value = segment.Replace('-', '+').Replace('_', '/');
        value = value.PadRight(value.Length + (4 - value.Length % 4) % 4, '=');
        return Convert.FromBase64String(value);
    }

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static DateTimeOffset? GetUnixTime(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        long seconds;

        if (!property.TryGetInt64(out seconds))
        {
            if (!property.TryGetDouble(out var doubleValue))
            {
                return null;
            }

            seconds = (long)doubleValue;
        }

        try
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }
}

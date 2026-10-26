namespace Seeker.DevToolbox.Core.Tools.Network;

public static class HttpCookieHelper
{
    public static IReadOnlyList<string> ExtractSetCookies(string headers)
    {
        var cookies = new List<string>();

        if (string.IsNullOrWhiteSpace(headers))
        {
            return cookies;
        }

        foreach (var rawLine in headers.Split('\n'))
        {
            var line = rawLine.Trim();

            if (!line.StartsWith("Set-Cookie:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = line["Set-Cookie:".Length..];
            var pair = value.Split(';')[0].Trim();

            if (pair.Contains('='))
            {
                cookies.Add(pair);
            }
        }

        return cookies;
    }

    public static string MergeCookieHeader(string existing, IEnumerable<string> cookies)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var part in existing.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separator = part.IndexOf('=');

            if (separator > 0)
            {
                values[part[..separator].Trim()] = part[(separator + 1)..].Trim();
            }
        }

        foreach (var cookie in cookies)
        {
            var separator = cookie.IndexOf('=');

            if (separator > 0)
            {
                values[cookie[..separator].Trim()] = cookie[(separator + 1)..].Trim();
            }
        }

        return string.Join("; ", values.Select(pair => $"{pair.Key}={pair.Value}"));
    }
}

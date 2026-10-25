using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace Seeker.DevToolbox.Core.Tools.Network;

public sealed record HttpToolRequest(
    string Method,
    string Url,
    string Headers = "",
    string Body = "",
    int TimeoutSeconds = 30);

public sealed record HttpToolResult(
    int StatusCode,
    string ReasonPhrase,
    string Headers,
    string Body,
    long ElapsedMilliseconds,
    long SizeBytes);

public static class HttpToolService
{
    private static readonly HttpClient Client = new(new HttpClientHandler { AllowAutoRedirect = true });

    static HttpToolService()
    {
        Client.Timeout = Timeout.InfiniteTimeSpan;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static async Task<HttpToolResult> SendAsync(
        HttpToolRequest options,
        CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(options.Url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("请输入有效的 http/https URL");
        }

        var methodName = string.IsNullOrWhiteSpace(options.Method)
            ? "GET"
            : options.Method.Trim().ToUpperInvariant();

        using var request = new HttpRequestMessage(new HttpMethod(methodName), uri);

        if (!string.IsNullOrEmpty(options.Body) && methodName is not ("GET" or "HEAD"))
        {
            request.Content = new StringContent(options.Body, Encoding.UTF8);
        }

        foreach (var header in ParseHeaders(options.Headers))
        {
            if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 1, 300)));

        var stopwatch = Stopwatch.StartNew();
        using var response = await Client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, timeoutSource.Token)
            .ConfigureAwait(false);

        var bytes = await response.Content.ReadAsByteArrayAsync(timeoutSource.Token).ConfigureAwait(false);
        stopwatch.Stop();

        var headers = string.Join(
            Environment.NewLine,
            response.Headers
                .Select(header => $"{header.Key}: {string.Join(", ", header.Value)}")
                .Concat(response.Content.Headers
                    .Select(header => $"{header.Key}: {string.Join(", ", header.Value)}")));

        return new HttpToolResult(
            StatusCode: (int)response.StatusCode,
            ReasonPhrase: response.ReasonPhrase ?? string.Empty,
            Headers: headers,
            Body: DecodeBody(bytes, response.Content.Headers.ContentType?.CharSet),
            ElapsedMilliseconds: stopwatch.ElapsedMilliseconds,
            SizeBytes: bytes.LongLength);
    }

    public static IReadOnlyList<KeyValuePair<string, string>> ParseHeaders(string text)
    {
        var headers = new List<KeyValuePair<string, string>>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return headers;
        }

        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var separator = line.IndexOf(':');

            if (separator <= 0)
            {
                throw new FormatException($"请求头格式应为 Name: Value,但收到:{line}");
            }

            var name = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();

            if (name.Length == 0)
            {
                throw new FormatException($"请求头名称不能为空:{line}");
            }

            headers.Add(new KeyValuePair<string, string>(name, value));
        }

        return headers;
    }

    private static string DecodeBody(byte[] bytes, string? charset)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        Encoding encoding = Encoding.UTF8;

        if (!string.IsNullOrWhiteSpace(charset))
        {
            try
            {
                encoding = Encoding.GetEncoding(charset.Trim().Trim('"', '\''));
            }
            catch (ArgumentException)
            {
                // 未知 charset 时回退 UTF-8。
            }
        }

        return encoding.GetString(bytes);
    }
}


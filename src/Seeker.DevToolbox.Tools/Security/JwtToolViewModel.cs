using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Security;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Security;

public partial class JwtToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _token = BuildSampleToken();

    [ObservableProperty]
    private string _headerJson = string.Empty;

    [ObservableProperty]
    private string _payloadJson = string.Empty;

    public JwtToolViewModel()
    {
        Parse();
    }

    [RelayCommand]
    private void Parse()
    {
        if (!JwtParser.TryParse(Token, out var info, out var error) || info is null)
        {
            HeaderJson = string.Empty;
            PayloadJson = string.Empty;
            SetStatus(error ?? "JWT 解析失败", true);
            return;
        }

        HeaderJson = info.HeaderJson;
        PayloadJson = info.PayloadJson;

        var now = DateTimeOffset.UtcNow;
        var expired = info.ExpiresAt is { } exp && exp <= now;
        SetStatus(BuildSummary(info, now), expired);
    }

    [RelayCommand]
    private void LoadSample()
    {
        Token = BuildSampleToken();
        Parse();
    }

    [RelayCommand]
    private void Clear()
    {
        Token = string.Empty;
        HeaderJson = string.Empty;
        PayloadJson = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void CopyPayload() => TryCopy(PayloadJson, "Payload 已复制");

    private static string BuildSummary(JwtTokenInfo info, DateTimeOffset now)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(info.Algorithm))
        {
            parts.Add($"alg={info.Algorithm}");
        }

        if (info.ExpiresAt is { } exp)
        {
            parts.Add(exp <= now
                ? $"已过期 {Humanize(now - exp)}({exp.ToLocalTime():yyyy-MM-dd HH:mm:ss})"
                : $"有效,剩余 {Humanize(exp - now)}({exp.ToLocalTime():yyyy-MM-dd HH:mm:ss})");
        }
        else
        {
            parts.Add("无 exp 声明");
        }

        if (info.NotBefore is { } nbf && nbf > now)
        {
            parts.Add($"尚未生效({nbf.ToLocalTime():yyyy-MM-dd HH:mm:ss})");
        }

        if (!info.HasSignature)
        {
            parts.Add("签名段为空");
        }

        return string.Join(" · ", parts);
    }

    private static string Humanize(TimeSpan span)
    {
        if (span.TotalDays >= 1)
        {
            return $"{(int)span.TotalDays} 天 {span.Hours} 小时";
        }

        if (span.TotalHours >= 1)
        {
            return $"{(int)span.TotalHours} 小时 {span.Minutes} 分";
        }

        if (span.TotalMinutes >= 1)
        {
            return $"{(int)span.TotalMinutes} 分 {span.Seconds} 秒";
        }

        return $"{(int)span.TotalSeconds} 秒";
    }

    private static string BuildSampleToken()
    {
        var header = Base64Url("""{"alg":"HS256","typ":"JWT"}""");
        var payload = Base64Url("""{"sub":"1234567890","name":"Seeker","role":"admin","iat":1767225600,"exp":1893456000}""");
        return $"{header}.{payload}.demo-signature";
    }

    private static string Base64Url(string text) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}

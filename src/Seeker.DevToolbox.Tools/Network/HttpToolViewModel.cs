using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Network;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Network;

public partial class HttpToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private int _methodIndex;

    [ObservableProperty]
    private string _url = "https://httpbin.org/get";

    [ObservableProperty]
    private string _headers = "Accept: application/json";

    [ObservableProperty]
    private string _body = string.Empty;

    [ObservableProperty]
    private string _cookieHeader = string.Empty;

    [ObservableProperty]
    private string _statusLine = "尚未发送请求";

    [ObservableProperty]
    private string _responseHeaders = string.Empty;

    [ObservableProperty]
    private string _responseBody = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private bool _isSending;

    [ObservableProperty]
    private HttpHistoryItem? _selectedHistory;

    public IReadOnlyList<string> Methods { get; } = ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD"];

    public ObservableCollection<HttpHistoryItem> History { get; } = [];

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendAsync()
    {
        IsSending = true;
        StatusLine = "请求发送中…";
        SetStatus("请求发送中…");
        AddHistory();

        try
        {
            var result = await HttpToolService.SendAsync(new HttpToolRequest(
                Methods[MethodIndex],
                Url,
                Headers,
                Body,
                Cookie: CookieHeader));

            StatusLine = $"{result.StatusCode} {result.ReasonPhrase} · {result.ElapsedMilliseconds} ms · {FormatSize(result.SizeBytes)}";
            ResponseHeaders = result.Headers;
            ResponseBody = result.Body;

            var setCookies = HttpCookieHelper.ExtractSetCookies(result.Headers);

            if (setCookies.Count > 0)
            {
                CookieHeader = HttpCookieHelper.MergeCookieHeader(CookieHeader, setCookies);
            }

            SetStatus($"请求完成 · HTTP {result.StatusCode}", result.StatusCode >= 400);
        }
        catch (OperationCanceledException)
        {
            StatusLine = "请求超时";
            SetStatus("请求超时", true);
        }
        catch (Exception ex)
        {
            StatusLine = "请求失败";
            ResponseHeaders = string.Empty;
            ResponseBody = string.Empty;
            SetStatus(ex.Message, true);
        }
        finally
        {
            IsSending = false;
        }
    }

    private bool CanSend() => !IsSending;

    [RelayCommand]
    private void CopyBody() => TryCopy(ResponseBody, "响应体已复制");

    [RelayCommand]
    private void ClearHistory()
    {
        History.Clear();
        SelectedHistory = null;
        SetStatus("历史已清空");
    }

    [RelayCommand]
    private void ClearCookie()
    {
        CookieHeader = string.Empty;
        SetStatus("Cookie 已清空");
    }

    [RelayCommand]
    private void Clear()
    {
        ResponseHeaders = string.Empty;
        ResponseBody = string.Empty;
        StatusLine = "尚未发送请求";
        SetStatus("已清空响应");
    }

    partial void OnSelectedHistoryChanged(HttpHistoryItem? value)
    {
        if (value is null)
        {
            return;
        }

        var index = Methods.ToList().IndexOf(value.Method);

        if (index >= 0)
        {
            MethodIndex = index;
        }

        Url = value.Url;
        Headers = value.Headers;
        Body = value.Body;
        CookieHeader = value.Cookie;
        SetStatus($"已载入历史:{value.Display}");
    }

    private void AddHistory()
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            return;
        }

        var item = new HttpHistoryItem(
            Methods[MethodIndex],
            Url,
            Headers,
            Body,
            CookieHeader,
            $"{Methods[MethodIndex]} {Url}");

        var existing = History.FirstOrDefault(entry =>
            string.Equals(entry.Method, item.Method, StringComparison.Ordinal)
            && string.Equals(entry.Url, item.Url, StringComparison.Ordinal)
            && string.Equals(entry.Body, item.Body, StringComparison.Ordinal)
            && string.Equals(entry.Headers, item.Headers, StringComparison.Ordinal));

        if (existing is not null)
        {
            History.Remove(existing);
        }

        History.Insert(0, item);
        SelectedHistory = item;

        while (History.Count > 20)
        {
            History.RemoveAt(History.Count - 1);
        }
    }

    private static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        var value = (double)bytes;
        var unit = 0;

        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.#} {units[unit]}";
    }
}

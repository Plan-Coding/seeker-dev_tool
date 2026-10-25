using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Time;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Time;

public partial class UnixTimeToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _input = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    [ObservableProperty]
    private int _unitIndex;

    [ObservableProperty]
    private string _localTime = string.Empty;

    [ObservableProperty]
    private string _utcTime = string.Empty;

    [ObservableProperty]
    private string _isoTime = string.Empty;

    [ObservableProperty]
    private string _seconds = string.Empty;

    [ObservableProperty]
    private string _milliseconds = string.Empty;

    public UnixTimeToolViewModel()
    {
        Convert();
    }

    public IReadOnlyList<string> Units { get; } = ["自动", "秒", "毫秒"];

    [RelayCommand]
    private void Convert()
    {
        var unit = UnitIndex switch
        {
            1 => UnixUnit.Seconds,
            2 => UnixUnit.Milliseconds,
            _ => UnixUnit.Auto
        };

        DateTimeOffset value;
        string note;

        if (UnixTime.TryParseTimestamp(Input, unit, out var timestamp, out var usedUnit, out var timestampError))
        {
            value = timestamp;
            note = usedUnit == UnixUnit.Milliseconds ? "按毫秒时间戳解析" : "按秒时间戳解析";
        }
        else if (UnixTime.TryParseDateTime(Input, out var dateTime, out _))
        {
            value = dateTime;
            note = "按日期时间解析";
        }
        else
        {
            SetStatus(timestampError ?? "无法解析输入", true);
            return;
        }

        LocalTime = UnixTime.FormatLocal(value);
        UtcTime = UnixTime.FormatUtc(value);
        IsoTime = UnixTime.FormatIso(value);
        Seconds = value.ToUnixTimeSeconds().ToString();
        Milliseconds = value.ToUnixTimeMilliseconds().ToString();

        SetStatus($"{note} · 本地时区 {TimeZoneInfo.Local.StandardName}");
    }

    [RelayCommand]
    private void Now()
    {
        Input = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        Convert();
        SetStatus("已填入当前时间");
    }

    [RelayCommand]
    private void CopyAll()
    {
        var text = $"""
            本地时间: {LocalTime}
            UTC 时间: {UtcTime}
            ISO 8601: {IsoTime}
            秒: {Seconds}
            毫秒: {Milliseconds}
            """;

        TryCopy(text, "全部结果已复制");
    }
}

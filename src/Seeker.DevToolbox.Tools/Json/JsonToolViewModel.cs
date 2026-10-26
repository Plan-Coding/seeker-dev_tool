using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Json;

namespace Seeker.DevToolbox.Tools.Json;

public partial class JsonToolViewModel : ObservableObject
{
    private const string SampleJson = """
        {
          "name": "Seeker DevToolbox",
          "portable": true,
          "tools": ["json", "base64", "timestamp"],
          "meta": { "version": "0.1.0", "author": "seeker" }
        }
        """;

    [ObservableProperty]
    private string _input = SampleJson;

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private string _status = "就绪";

    [ObservableProperty]
    private bool _hasError;

    public JsonToolViewModel()
    {
        Run(JsonIndentStyle.TwoSpaces);
    }

    [RelayCommand]
    private void Format() => Run(JsonIndentStyle.TwoSpaces);

    [RelayCommand]
    private void Minify() => Run(JsonIndentStyle.None);

    [RelayCommand]
    private void FormatJson5()
    {
        if (!Json5Converter.TryToJson(Input, out var result, out var error))
        {
            Status = error ?? "JSON5 格式化失败";
            HasError = true;
            return;
        }

        Output = result;
        Status = $"JSON5 → JSON 完成 · {result.Length:N0} 字符";
        HasError = false;
    }

    [RelayCommand]
    private void LoadSample()
    {
        Input = SampleJson;
        Run(JsonIndentStyle.TwoSpaces);
    }

    [RelayCommand]
    private void Clear()
    {
        Input = string.Empty;
        Output = string.Empty;
        Status = "已清空";
        HasError = false;
    }

    [RelayCommand]
    private void CopyResult()
    {
        if (string.IsNullOrEmpty(Output))
        {
            return;
        }

        try
        {
            System.Windows.Clipboard.SetText(Output);
            Status = "结果已复制到剪贴板";
            HasError = false;
        }
        catch (Exception ex)
        {
            Status = $"复制失败:{ex.Message}";
            HasError = true;
        }
    }

    private void Run(JsonIndentStyle style)
    {
        if (string.IsNullOrWhiteSpace(Input))
        {
            Output = string.Empty;
            Status = "请输入 JSON 文本";
            HasError = false;
            return;
        }

        if (!JsonFormatter.TryFormat(Input, style, out var result, out var error))
        {
            Output = string.Empty;
            Status = error ?? "格式化失败";
            HasError = true;
            return;
        }

        Output = result;
        Status = $"{(style == JsonIndentStyle.None ? "已压缩" : "格式化成功")} · {result.Length:N0} 字符";
        HasError = false;
    }
}

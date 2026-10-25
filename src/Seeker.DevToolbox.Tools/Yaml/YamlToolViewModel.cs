using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Yaml;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Yaml;

public partial class YamlToolViewModel : ToolViewModelBase
{
    private const string SampleYaml = """
        name: Seeker DevToolbox
        portable: true
        tools:
          - json
          - diff
          - http
        meta:
          version: 0.4.0
        """;

    [ObservableProperty]
    private string _input = SampleYaml;

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private string _outputHighlighting = "Json";

    public YamlToolViewModel()
    {
        YamlToJson();
    }

    [RelayCommand]
    private void YamlToJson()
    {
        if (!YamlJsonConverter.TryYamlToJson(Input, out var result, out var error))
        {
            SetStatus(error ?? "YAML 转 JSON 失败", true);
            return;
        }

        Output = result;
        OutputHighlighting = "Json";
        SetStatus($"YAML → JSON 完成 · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void JsonToYaml()
    {
        if (!YamlJsonConverter.TryJsonToYaml(Input, out var result, out var error))
        {
            SetStatus(error ?? "JSON 转 YAML 失败", true);
            return;
        }

        Output = result;
        OutputHighlighting = "";
        SetStatus($"JSON → YAML 完成 · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void FormatYaml()
    {
        if (!YamlJsonConverter.TryFormatYaml(Input, out var result, out var error))
        {
            SetStatus(error ?? "YAML 格式化失败", true);
            return;
        }

        Output = result;
        OutputHighlighting = "";
        SetStatus($"YAML 格式化完成 · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void LoadSample()
    {
        Input = SampleYaml;
        YamlToJson();
    }

    [RelayCommand]
    private void Clear()
    {
        Input = string.Empty;
        Output = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy() => TryCopy(Output);
}

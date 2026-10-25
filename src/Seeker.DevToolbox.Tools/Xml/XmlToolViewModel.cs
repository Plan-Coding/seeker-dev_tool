using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Xml;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Xml;

public partial class XmlToolViewModel : ToolViewModelBase
{
    private const string SampleXml = """
        <?xml version="1.0" encoding="utf-8"?>
        <catalog>
          <tool id="json"><name>JSON 格式化</name></tool>
          <tool id="diff"><name>文本 Diff</name></tool>
        </catalog>
        """;

    [ObservableProperty]
    private string _input = SampleXml;

    [ObservableProperty]
    private string _output = string.Empty;

    public XmlToolViewModel()
    {
        Format();
    }

    [RelayCommand]
    private void Format()
    {
        if (XmlFormatter.TryFormat(Input, out var result, out var error))
        {
            Output = result;
            SetStatus($"格式化完成 · {result.Length:N0} 字符");
        }
        else
        {
            SetStatus(error ?? "格式化失败", true);
        }
    }

    [RelayCommand]
    private void Minify()
    {
        if (XmlFormatter.TryMinify(Input, out var result, out var error))
        {
            Output = result;
            SetStatus($"压缩完成 · {result.Length:N0} 字符");
        }
        else
        {
            SetStatus(error ?? "压缩失败", true);
        }
    }

    [RelayCommand]
    private void LoadSample()
    {
        Input = SampleXml;
        Format();
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

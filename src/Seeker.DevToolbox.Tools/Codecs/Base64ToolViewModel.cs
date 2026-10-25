using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Codecs;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Codecs;

public partial class Base64ToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _input = "Seeker DevToolbox";

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private bool _urlSafe;

    public Base64ToolViewModel()
    {
        Encode();
    }

    [RelayCommand]
    private void Encode()
    {
        RunSafely(() =>
        {
            Output = Base64Codec.Encode(Input, UrlSafe);
            SetStatus($"编码完成 · {Output.Length:N0} 字符");
        });
    }

    [RelayCommand]
    private void Decode()
    {
        if (Base64Codec.TryDecode(Input, out var result, out var error))
        {
            Output = result;
            SetStatus($"解码完成 · {result.Length:N0} 字符");
        }
        else
        {
            SetStatus(error ?? "解码失败", true);
        }
    }

    [RelayCommand]
    private void Swap()
    {
        (Input, Output) = (Output, Input);
        SetStatus("已交换输入与输出");
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

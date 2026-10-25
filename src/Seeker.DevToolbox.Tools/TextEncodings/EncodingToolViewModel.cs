using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.TextEncodings;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.TextEncodings;

public partial class EncodingToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private int _sourceIndex = 1;

    [ObservableProperty]
    private int _targetIndex;

    [ObservableProperty]
    private string _input = "浣犲ソ";

    [ObservableProperty]
    private string _output = string.Empty;

    public EncodingToolViewModel()
    {
        Convert();
    }

    public IReadOnlyList<EncodingOption> Encodings => TextEncodingConverter.Options;

    [RelayCommand]
    private void Convert()
    {
        var source = Encodings[SourceIndex];
        var target = Encodings[TargetIndex];

        if (!TextEncodingConverter.TryConvert(Input, source.Id, target.Id, out var result, out var error))
        {
            SetStatus(error ?? "编码转换失败", true);
            return;
        }

        Output = result;
        SetStatus($"{source.DisplayName} → {target.DisplayName} · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void Swap()
    {
        (SourceIndex, TargetIndex) = (TargetIndex, SourceIndex);
        Convert();
    }

    [RelayCommand]
    private void LoadSample()
    {
        SourceIndex = 1;
        TargetIndex = 0;
        Input = "浣犲ソ";
        Convert();
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


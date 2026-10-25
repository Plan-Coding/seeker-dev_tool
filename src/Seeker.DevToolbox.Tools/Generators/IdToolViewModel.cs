using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Generators;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Generators;

public partial class IdToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private int _versionIndex = 1;

    [ObservableProperty]
    private string _countText = "10";

    [ObservableProperty]
    private bool _uppercase;

    [ObservableProperty]
    private bool _includeHyphens = true;

    [ObservableProperty]
    private string _output = string.Empty;

    public IdToolViewModel()
    {
        Generate();
    }

    public IReadOnlyList<string> Versions { get; } = ["UUID v4(随机)", "UUID v7(按时间)"];

    [RelayCommand]
    private void Generate()
    {
        if (!int.TryParse(CountText, out var count))
        {
            SetStatus("数量必须是整数", true);
            return;
        }

        var lines = IdGenerator.Generate(
            count,
            version7: VersionIndex == 1,
            uppercase: Uppercase,
            includeHyphens: IncludeHyphens);

        Output = string.Join(Environment.NewLine, lines);
        SetStatus($"已生成 {lines.Count} 个 {(VersionIndex == 1 ? "UUID v7" : "UUID v4")}");
    }

    [RelayCommand]
    private void Clear()
    {
        Output = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy() => TryCopy(Output);
}

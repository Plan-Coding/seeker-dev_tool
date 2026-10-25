using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Text;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Text;

public partial class DiffToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _oldText = "第一行\n第二行\n第三行";

    [ObservableProperty]
    private string _newText = "第一行\n第二行(修改)\n第三行\n新增一行";

    [ObservableProperty]
    private bool _ignoreWhitespace = true;

    public DiffToolViewModel()
    {
        Compare();
    }

    public ObservableCollection<DiffLineItem> Lines { get; } = [];

    [RelayCommand]
    private void Compare()
    {
        var result = TextDiffService.Compare(OldText, NewText, IgnoreWhitespace);

        Lines.Clear();
        foreach (var line in result.Lines)
        {
            Lines.Add(DiffLineItem.From(line));
        }

        var parts = new List<string>();

        if (result.AddedCount > 0)
        {
            parts.Add($"+{result.AddedCount} 行");
        }

        if (result.RemovedCount > 0)
        {
            parts.Add($"-{result.RemovedCount} 行");
        }

        if (result.ModifiedCount > 0)
        {
            parts.Add($"~{result.ModifiedCount} 行");
        }

        SetStatus(parts.Count == 0 ? "两段文本完全相同" : string.Join(" · ", parts));
    }

    [RelayCommand]
    private void Swap()
    {
        (OldText, NewText) = (NewText, OldText);
        Compare();
        SetStatus($"已交换 · {Status}");
    }

    [RelayCommand]
    private void Clear()
    {
        Lines.Clear();
        OldText = string.Empty;
        NewText = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy()
    {
        var text = string.Join(
            Environment.NewLine,
            Lines.Select(line => $"{line.Prefix} {line.Text}"));

        TryCopy(text, "Diff 结果已复制");
    }
}

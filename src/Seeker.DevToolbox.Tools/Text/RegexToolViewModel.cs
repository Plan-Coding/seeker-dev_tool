using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Text;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Text;

public partial class RegexToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _pattern = @"(\w+)@(\w+)\.com";

    [ObservableProperty]
    private string _input = "alice@example.com\nbob@test.com\nnot-an-email";

    [ObservableProperty]
    private string _replacement = "$1 at $2";

    [ObservableProperty]
    private string _replaceOutput = string.Empty;

    [ObservableProperty]
    private bool _ignoreCase = true;

    [ObservableProperty]
    private bool _multiline;

    [ObservableProperty]
    private bool _singleline;

    public RegexToolViewModel()
    {
        Match();
        Replace();
    }

    public ObservableCollection<RegexMatchItem> Matches { get; } = [];

    [RelayCommand]
    private void Match()
    {
        Matches.Clear();

        if (!RegexTool.TryMatches(Pattern, Input, BuildOptions(), 500, out var matches, out var error))
        {
            SetStatus(error ?? "匹配失败", true);
            return;
        }

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var groups = string.Join(
                "  ",
                match.Groups
                    .Skip(1)
                    .Where(group => group.Success)
                    .Select(group =>
                        group.Name == group.Number.ToString()
                            ? $"${group.Number}={group.Value}"
                            : $"${group.Number}<{group.Name}>={group.Value}"));

            Matches.Add(new RegexMatchItem(
                $"#{i + 1}  [{match.Index}..{match.Index + match.Length}]  {match.Value}",
                groups));
        }

        SetStatus(matches.Count == 0 ? "没有匹配" : $"找到 {matches.Count} 处匹配");
    }

    [RelayCommand]
    private void Replace()
    {
        if (!RegexTool.TryReplace(Pattern, Input, Replacement, BuildOptions(), out var result, out var error))
        {
            SetStatus(error ?? "替换失败", true);
            return;
        }

        ReplaceOutput = result;
        SetStatus($"替换完成 · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void Clear()
    {
        Matches.Clear();
        ReplaceOutput = string.Empty;
        SetStatus("已清空结果");
    }

    private RegexOptions BuildOptions()
    {
        var options = RegexOptions.None;

        if (IgnoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        if (Multiline)
        {
            options |= RegexOptions.Multiline;
        }

        if (Singleline)
        {
            options |= RegexOptions.Singleline;
        }

        return options;
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.SystemTools;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.SystemTools;

public partial class ProcessToolViewModel : ToolViewModelBase
{
    private IReadOnlyList<ProcessItem> _all = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    public ProcessToolViewModel()
    {
        Refresh();
    }

    public ObservableCollection<ProcessItem> Processes { get; } = [];

    [RelayCommand]
    private void Refresh()
    {
        _all = ProcessScanner.GetProcesses()
            .Select(entry => new ProcessItem(
                entry.Pid,
                entry.Name,
                FormatMemory(entry.WorkingSetBytes),
                entry.StartTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "-",
                entry.FilePath ?? "-"))
            .ToList();

        ApplyFilter();
        SetStatus($"共 {_all.Count} 个进程");
    }

    [RelayCommand]
    private void Copy()
    {
        var text = string.Join(
            Environment.NewLine,
            Processes.Select(item => $"{item.Pid}\t{item.Name}\t{item.Memory}\t{item.StartTime}\t{item.FilePath}"));

        TryCopy(text, "进程列表已复制");
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = SearchText.Trim();

        var matches = string.IsNullOrEmpty(query)
            ? _all
            : _all.Where(item =>
                    item.Pid.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)
                    || item.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || item.FilePath.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

        Processes.Clear();
        foreach (var item in matches)
        {
            Processes.Add(item);
        }

        SetStatus($"显示 {matches.Count} / {_all.Count} 个进程");
    }

    private static string FormatMemory(long bytes)
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

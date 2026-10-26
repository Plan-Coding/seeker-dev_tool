using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
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

    [ObservableProperty]
    private ProcessItem? _selectedProcess;

    public ProcessToolViewModel()
    {
        Refresh();
    }

    public ObservableCollection<ProcessItem> Processes { get; } = [];

    [RelayCommand]
    private void Refresh()
    {
        SelectedProcess = null;

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

    [RelayCommand(CanExecute = nameof(CanActOnProcess))]
    private void OpenLocation()
    {
        var target = SelectedProcess;

        if (target is null || string.IsNullOrWhiteSpace(target.FilePath) || target.FilePath == "-")
        {
            SetStatus("该进程没有可用的文件路径", true);
            return;
        }

        if (!File.Exists(target.FilePath))
        {
            SetStatus("可执行文件已不存在", true);
            return;
        }

        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{target.FilePath}\"")
        {
            UseShellExecute = true
        });

        SetStatus($"已在资源管理器中定位:{Path.GetFileName(target.FilePath)}");
    }

    [RelayCommand(CanExecute = nameof(CanActOnProcess))]
    private void KillProcess()
    {
        var target = SelectedProcess;

        if (target is null)
        {
            return;
        }

        if (target.Pid == Environment.ProcessId)
        {
            SetStatus("不能结束当前程序", true);
            return;
        }

        var confirm = MessageBox.Show(
            $"确定结束进程 {target.Name}(PID {target.Pid})?\n未保存的数据可能丢失。",
            "结束进程",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var process = Process.GetProcessById(target.Pid);
            process.Kill();
            SetStatus($"已结束 {target.Name}(PID {target.Pid})");
            Refresh();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
        {
            SetStatus($"结束失败:{ex.Message}", true);
        }
    }

    private bool CanActOnProcess() => SelectedProcess is not null;

    partial void OnSelectedProcessChanged(ProcessItem? value)
    {
        OpenLocationCommand.NotifyCanExecuteChanged();
        KillProcessCommand.NotifyCanExecuteChanged();
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

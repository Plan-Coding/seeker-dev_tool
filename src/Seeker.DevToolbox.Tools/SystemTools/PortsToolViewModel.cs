using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.SystemTools;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.SystemTools;

public partial class PortsToolViewModel : ToolViewModelBase
{
    private IReadOnlyList<PortEntry> _all = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    public PortsToolViewModel()
    {
        Refresh();
    }

    public ObservableCollection<PortEntry> Ports { get; } = [];

    [RelayCommand]
    private void Refresh()
    {
        _all = PortScanner.GetListeningTcpPorts();
        ApplyFilter();
        SetStatus(_all.Count == 0 ? "没有找到 TCP 监听端口" : $"找到 {_all.Count} 个 TCP 监听端口");
    }

    [RelayCommand]
    private void Copy()
    {
        var text = string.Join(
            Environment.NewLine,
            Ports.Select(entry => $"{entry.Port}\t{entry.LocalAddress}\t{entry.Pid}\t{entry.ProcessName}"));

        TryCopy(text, "端口列表已复制");
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = SearchText.Trim();

        var matches = string.IsNullOrEmpty(query)
            ? _all
            : _all.Where(entry =>
                    entry.Port.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)
                    || entry.Pid.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)
                    || entry.LocalAddress.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || entry.ProcessName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

        Ports.Clear();
        foreach (var entry in matches)
        {
            Ports.Add(entry);
        }

        SetStatus($"显示 {matches.Count} / {_all.Count} 个监听端口");
    }
}

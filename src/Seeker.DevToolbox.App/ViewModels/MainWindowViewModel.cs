using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Seeker.DevToolbox.Core.Configuration;
using Seeker.DevToolbox.UI.Appearance;
using Seeker.DevToolbox.UI.Tools;

namespace Seeker.DevToolbox.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private const int MaxRecentTools = 8;

    private readonly List<ToolItem> _allTools;
    private readonly SettingsService _settings;
    private readonly IThemeService _theme;
    private readonly AppPaths _paths;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ToolItem? _selectedTool;

    [ObservableProperty]
    private string _themeLabel = string.Empty;

    public MainWindowViewModel(
        ToolRegistry registry,
        SettingsService settings,
        IThemeService theme,
        AppPaths paths)
    {
        _settings = settings;
        _theme = theme;
        _paths = paths;
        _allTools = registry.All.Select(module => new ToolItem(module)).ToList();

        var savedTheme = Enum.TryParse<AppTheme>(settings.Current.Theme, ignoreCase: true, out var parsed)
            ? parsed
            : AppTheme.System;

        _theme.Apply(savedTheme);
        ThemeLabel = _theme.DisplayName;

        RefreshFilter();

        SelectedTool = _allTools.FirstOrDefault(item =>
                           string.Equals(item.Id, settings.Current.LastToolId, StringComparison.OrdinalIgnoreCase))
                       ?? _allTools.FirstOrDefault();
    }

    public ObservableCollection<ToolItem> VisibleTools { get; } = [];

    public string Version => typeof(MainWindowViewModel).Assembly
        .GetName().Version?.ToString(3) ?? "0.1.0";

    public string DataRoot => _paths.DataRoot;

    partial void OnSearchTextChanged(string value) => RefreshFilter();

    public void NotifyOpened(ToolItem item)
    {
        SelectedTool = item;
        _settings.Current.LastToolId = item.Id;

        var recent = _settings.Current.RecentToolIds;
        recent.RemoveAll(id => string.Equals(id, item.Id, StringComparison.OrdinalIgnoreCase));
        recent.Insert(0, item.Id);

        if (recent.Count > MaxRecentTools)
        {
            recent.RemoveRange(MaxRecentTools, recent.Count - MaxRecentTools);
        }

        _settings.TrySave(out _);
    }

    public void CycleTheme()
    {
        var next = _theme.Cycle();
        ThemeLabel = _theme.DisplayName;

        _settings.Current.Theme = next.ToString();
        _settings.TrySave(out _);
    }

    private void RefreshFilter()
    {
        var matches = _allTools.Where(item => item.Matches(SearchText)).ToList();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            var recent = _settings.Current.RecentToolIds;

            matches = matches
                .OrderBy(item =>
                {
                    var index = recent.IndexOf(item.Id);
                    return index >= 0 ? index : int.MaxValue;
                })
                .ThenBy(item => _allTools.IndexOf(item))
                .ToList();
        }

        VisibleTools.Clear();
        foreach (var item in matches)
        {
            VisibleTools.Add(item);
        }

        if (SelectedTool is not null && !VisibleTools.Contains(SelectedTool))
        {
            SelectedTool = VisibleTools.FirstOrDefault();
        }
    }
}

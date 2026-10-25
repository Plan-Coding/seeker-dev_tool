using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Seeker.DevToolbox.App.ViewModels;
using Seeker.DevToolbox.Core.Configuration;
using Seeker.DevToolbox.UI.Appearance;
using Seeker.DevToolbox.UI.Tools;
using Serilog;
using Wpf.Ui.Controls;

namespace Seeker.DevToolbox.App;

public partial class MainWindow : FluentWindow
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IServiceProvider _services;
    private readonly IThemeService _themeService;
    private readonly SettingsService _settings;
    private readonly Dictionary<string, FrameworkElement> _views = new();

    private string? _currentToolId;
    private bool _isRebuildingMenu;

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider services,
        IThemeService themeService,
        SettingsService settings)
    {
        _viewModel = viewModel;
        _services = services;
        _themeService = themeService;
        _settings = settings;

        InitializeComponent();
        DataContext = viewModel;

        RestoreWindowPlacement();

        _viewModel.VisibleTools.CollectionChanged += OnVisibleToolsChanged;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        Nav.ItemInvoked += OnNavItemInvoked;

        RebuildMenu();
        UpdateThemeIcon();

        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _themeService.Attach(this);
        UpdateThemeIcon();
        SelectTool(_viewModel.SelectedTool);
        SearchBox.Focus();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        _settings.Current.WindowMaximized = WindowState == WindowState.Maximized;

        if (WindowState == WindowState.Normal)
        {
            _settings.Current.WindowWidth = Width;
            _settings.Current.WindowHeight = Height;
        }

        if (!_settings.TrySave(out var error))
        {
            Log.Warning("Failed to save settings: {Error}", error);
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
            e.Handled = true;
        }

        base.OnPreviewKeyDown(e);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.SelectedTool)
            && _viewModel.SelectedTool is { } tool)
        {
            SelectTool(tool);
        }
    }

    private void OnVisibleToolsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildMenu();
    }

    private void OnNavItemInvoked(NavigationView sender, RoutedEventArgs args)
    {
        if (_isRebuildingMenu)
        {
            return;
        }

        if (sender.SelectedItem is NavigationViewItem { Tag: string id })
        {
            SelectTool(_viewModel.VisibleTools.FirstOrDefault(item => item.Id == id));
        }
    }

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || _viewModel.VisibleTools.Count == 0)
        {
            return;
        }

        SelectTool(_viewModel.VisibleTools[0]);
        e.Handled = true;
    }

    private void OnThemeClick(object sender, RoutedEventArgs e)
    {
        _viewModel.CycleTheme();
        UpdateThemeIcon();
    }

    private void UpdateThemeIcon()
    {
        ThemeIcon.Symbol = _themeService.Current switch
        {
            AppTheme.Light => SymbolRegular.WeatherSunny24,
            AppTheme.Dark => SymbolRegular.WeatherMoon24,
            _ => SymbolRegular.Desktop24
        };
    }

    private void SelectTool(ToolItem? item)
    {
        if (item is null || _currentToolId == item.Id)
        {
            return;
        }

        _currentToolId = item.Id;

        if (!_views.TryGetValue(item.Id, out var view))
        {
            try
            {
                view = item.Module.CreateView(_services);
                _views[item.Id] = view;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create view for tool {ToolId}", item.Id);

                view = new System.Windows.Controls.TextBlock
                {
                    Margin = new Thickness(32),
                    TextWrapping = TextWrapping.Wrap,
                    Text = $"工具“{item.Title}”加载失败:{ex.Message}\n\n详细信息已写入日志。"
                };
            }
        }

        ContentHost.Content = view;
        _viewModel.SelectedTool = item;
        _viewModel.NotifyOpened(item);
        HighlightMenuItem(item.Id);
    }

    private void RebuildMenu()
    {
        _isRebuildingMenu = true;

        try
        {
            Nav.MenuItems.Clear();

            foreach (var item in _viewModel.VisibleTools)
            {
                Nav.MenuItems.Add(new NavigationViewItem
                {
                    Content = item.Title,
                    Tag = item.Id,
                    Icon = new SymbolIcon { Symbol = item.Symbol }
                });
            }
        }
        finally
        {
            _isRebuildingMenu = false;
        }

        if (_currentToolId is not null)
        {
            HighlightMenuItem(_currentToolId);
        }
    }

    private void HighlightMenuItem(string toolId)
    {
        NavigationViewItem? target = null;

        foreach (var rawItem in Nav.MenuItems)
        {
            if (rawItem is not NavigationViewItem navItem)
            {
                continue;
            }

            navItem.IsActive = false;

            if (navItem.Tag is string id
                && string.Equals(id, toolId, StringComparison.OrdinalIgnoreCase))
            {
                target = navItem;
            }
        }

        if (target is not null)
        {
            target.IsActive = true;
        }
    }

    private void RestoreWindowPlacement()
    {
        Width = Math.Clamp(_settings.Current.WindowWidth, MinWidth, SystemParameters.WorkArea.Width);
        Height = Math.Clamp(_settings.Current.WindowHeight, MinHeight, SystemParameters.WorkArea.Height);

        if (_settings.Current.WindowMaximized)
        {
            WindowState = WindowState.Maximized;
        }
    }
}

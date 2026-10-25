using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Seeker.DevToolbox.UI.Appearance;

/// <summary>
/// 基于 WPF-UI 的主题服务。System 模式会监听系统明暗切换。
/// </summary>
public sealed class ThemeService : IThemeService
{
    private static readonly WindowBackdropType Backdrop = WindowBackdropType.Mica;

    private Window? _window;

    public AppTheme Current { get; private set; } = AppTheme.System;

    public string DisplayName => Current switch
    {
        AppTheme.Dark => "深色",
        AppTheme.Light => "浅色",
        _ => "跟随系统"
    };

    public void Attach(Window window)
    {
        _window = window;
        Apply(Current);
    }

    public void Apply(AppTheme theme)
    {
        Current = theme;

        if (_window is not null)
        {
            SystemThemeWatcher.UnWatch(_window);
        }

        switch (theme)
        {
            case AppTheme.Light:
                ApplicationThemeManager.Apply(ApplicationTheme.Light, Backdrop, true);
                break;

            case AppTheme.Dark:
                ApplicationThemeManager.Apply(ApplicationTheme.Dark, Backdrop, true);
                break;

            default:
                var systemTheme = ApplicationThemeManager.GetSystemTheme();
                ApplicationThemeManager.Apply(
                    systemTheme == SystemTheme.Light ? ApplicationTheme.Light : ApplicationTheme.Dark,
                    Backdrop,
                    true);

                if (_window is not null)
                {
                    SystemThemeWatcher.Watch(_window, Backdrop, true);
                }

                break;
        }
    }

    public AppTheme Cycle()
    {
        var next = Current switch
        {
            AppTheme.System => AppTheme.Dark,
            AppTheme.Dark => AppTheme.Light,
            _ => AppTheme.System
        };

        Apply(next);
        return next;
    }
}

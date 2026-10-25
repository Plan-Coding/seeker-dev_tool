using System.Windows;

namespace Seeker.DevToolbox.UI.Appearance;

public enum AppTheme
{
    System,
    Dark,
    Light
}

public interface IThemeService
{
    AppTheme Current { get; }

    /// <summary>绑定主窗口,用于跟随系统主题变化。</summary>
    void Attach(Window window);

    void Apply(AppTheme theme);

    AppTheme Cycle();

    string DisplayName { get; }
}

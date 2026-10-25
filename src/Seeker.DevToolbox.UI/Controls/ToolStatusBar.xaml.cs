using System.Windows;
using System.Windows.Controls;

namespace Seeker.DevToolbox.UI.Controls;

public partial class ToolStatusBar : UserControl
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(ToolStatusBar),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
        nameof(HasError),
        typeof(bool),
        typeof(ToolStatusBar),
        new PropertyMetadata(false));

    public ToolStatusBar()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool HasError
    {
        get => (bool)GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }
}

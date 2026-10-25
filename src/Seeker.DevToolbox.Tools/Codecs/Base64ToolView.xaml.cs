using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Codecs;

public partial class Base64ToolView : UserControl
{
    public Base64ToolView()
    {
        InitializeComponent();
        DataContext = new Base64ToolViewModel();
    }
}

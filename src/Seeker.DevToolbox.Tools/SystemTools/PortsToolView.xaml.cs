using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.SystemTools;

public partial class PortsToolView : UserControl
{
    public PortsToolView()
    {
        InitializeComponent();
        DataContext = new PortsToolViewModel();
    }
}

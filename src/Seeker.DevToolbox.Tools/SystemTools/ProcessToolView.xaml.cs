using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.SystemTools;

public partial class ProcessToolView : UserControl
{
    public ProcessToolView()
    {
        InitializeComponent();
        DataContext = new ProcessToolViewModel();
    }
}

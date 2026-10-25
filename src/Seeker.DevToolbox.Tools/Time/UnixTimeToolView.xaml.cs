using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Time;

public partial class UnixTimeToolView : UserControl
{
    public UnixTimeToolView()
    {
        InitializeComponent();
        DataContext = new UnixTimeToolViewModel();
    }
}

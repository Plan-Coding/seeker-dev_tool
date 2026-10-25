using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Text;

public partial class DiffToolView : UserControl
{
    public DiffToolView()
    {
        InitializeComponent();
        DataContext = new DiffToolViewModel();
    }
}

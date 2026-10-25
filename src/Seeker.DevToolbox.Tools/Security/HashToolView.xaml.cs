using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Security;

public partial class HashToolView : UserControl
{
    public HashToolView()
    {
        InitializeComponent();
        DataContext = new HashToolViewModel();
    }
}

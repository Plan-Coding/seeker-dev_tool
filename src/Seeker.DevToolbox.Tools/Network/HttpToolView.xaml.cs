using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Network;

public partial class HttpToolView : UserControl
{
    public HttpToolView()
    {
        InitializeComponent();
        DataContext = new HttpToolViewModel();
    }
}

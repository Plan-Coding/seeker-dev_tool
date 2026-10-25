using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Codecs;

public partial class UrlToolView : UserControl
{
    public UrlToolView()
    {
        InitializeComponent();
        DataContext = new UrlToolViewModel();
    }
}

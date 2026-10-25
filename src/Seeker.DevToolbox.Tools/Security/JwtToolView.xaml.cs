using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Security;

public partial class JwtToolView : UserControl
{
    public JwtToolView()
    {
        InitializeComponent();
        DataContext = new JwtToolViewModel();
    }
}

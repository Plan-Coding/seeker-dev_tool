using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Text;

public partial class RegexToolView : UserControl
{
    public RegexToolView()
    {
        InitializeComponent();
        DataContext = new RegexToolViewModel();
    }
}

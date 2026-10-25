using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Design;

public partial class ColorToolView : UserControl
{
    public ColorToolView()
    {
        InitializeComponent();
        DataContext = new ColorToolViewModel();
    }
}

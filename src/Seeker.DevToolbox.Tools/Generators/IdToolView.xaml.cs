using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Generators;

public partial class IdToolView : UserControl
{
    public IdToolView()
    {
        InitializeComponent();
        DataContext = new IdToolViewModel();
    }
}

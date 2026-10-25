using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Text;

public partial class TextToolView : UserControl
{
    public TextToolView()
    {
        InitializeComponent();
        DataContext = new TextToolViewModel();
    }
}

using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.TextEncodings;

public partial class EncodingToolView : UserControl
{
    public EncodingToolView()
    {
        InitializeComponent();
        DataContext = new EncodingToolViewModel();
    }
}


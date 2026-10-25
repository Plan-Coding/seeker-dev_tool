using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Welcome;

public partial class WelcomeView : UserControl
{
    public WelcomeView(WelcomeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Yaml;

public partial class YamlToolView : UserControl
{
    public YamlToolView()
    {
        InitializeComponent();
        DataContext = new YamlToolViewModel();
    }
}

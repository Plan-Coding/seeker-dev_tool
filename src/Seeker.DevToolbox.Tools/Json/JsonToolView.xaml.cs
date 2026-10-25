using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Json;

public partial class JsonToolView : UserControl
{
    public JsonToolView()
    {
        InitializeComponent();
        DataContext = new JsonToolViewModel();
    }
}

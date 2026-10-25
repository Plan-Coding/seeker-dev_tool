using System.Windows.Controls;

namespace Seeker.DevToolbox.Tools.Xml;

public partial class XmlToolView : UserControl
{
    public XmlToolView()
    {
        InitializeComponent();
        DataContext = new XmlToolViewModel();
    }
}

using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Design;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Design;

public partial class ColorToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _input = "#3A6EEB";

    [ObservableProperty]
    private string _hex = string.Empty;

    [ObservableProperty]
    private string _rgb = string.Empty;

    [ObservableProperty]
    private string _hsl = string.Empty;

    [ObservableProperty]
    private string _hsv = string.Empty;

    [ObservableProperty]
    private Brush _previewBrush = Brushes.Transparent;

    public ColorToolViewModel()
    {
        Convert();
    }

    [RelayCommand]
    private void Convert()
    {
        if (!ColorTool.TryParse(Input, out var color, out var error))
        {
            SetStatus(error ?? "颜色解析失败", true);
            return;
        }

        Hex = ColorTool.ToHex(color);
        Rgb = ColorTool.ToRgbString(color);
        Hsl = ColorTool.ToHslString(color);
        Hsv = ColorTool.ToHsvString(color);
        PreviewBrush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));

        SetStatus($"已解析:{Hex}");
    }

    [RelayCommand]
    private void CopyHex() => TryCopy(Hex, "HEX 已复制");
}

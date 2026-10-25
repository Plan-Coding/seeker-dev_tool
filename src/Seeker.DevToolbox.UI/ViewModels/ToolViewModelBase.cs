using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Seeker.DevToolbox.UI.ViewModels;

/// <summary>
/// 工具 ViewModel 的公共基类:统一状态栏、错误标记和剪贴板操作。
/// </summary>
public abstract partial class ToolViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _status = "就绪";

    [ObservableProperty]
    private bool _hasError;

    protected void SetStatus(string status, bool isError = false)
    {
        Status = status;
        HasError = isError;
    }

    protected bool TryCopy(string? text, string successMessage = "已复制到剪贴板")
    {
        if (string.IsNullOrEmpty(text))
        {
            SetStatus("没有可复制的内容");
            return false;
        }

        try
        {
            Clipboard.SetText(text);
            SetStatus(successMessage);
            return true;
        }
        catch (Exception ex)
        {
            SetStatus($"复制失败:{ex.Message}", true);
            return false;
        }
    }

    protected void RunSafely(Action action, string? successMessage = null)
    {
        try
        {
            action();

            if (successMessage is not null)
            {
                SetStatus(successMessage);
            }
        }
        catch (Exception ex)
        {
            SetStatus(ex.Message, true);
        }
    }
}

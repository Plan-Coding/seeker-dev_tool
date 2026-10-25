using System.Windows;
using Seeker.DevToolbox.Core.Tools;

namespace Seeker.DevToolbox.UI.Contracts;

/// <summary>
/// 一个工具模块 = 元数据 + 视图工厂。视图按需创建并缓存。
/// </summary>
public interface IToolModule
{
    ToolMetadata Metadata { get; }

    FrameworkElement CreateView(IServiceProvider services);
}

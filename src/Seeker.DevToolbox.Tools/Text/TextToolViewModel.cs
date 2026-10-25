using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Text;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Text;

public partial class TextToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private string _input = "user_name\nHTTPServer\n  order-id\nuser_name";

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private bool _ignoreCase = true;

    [ObservableProperty]
    private bool _trimBeforeDedupe = true;

    public TextToolViewModel()
    {
        ToCamelCase();
    }

    [RelayCommand]
    private void ToCamelCase() => ApplyPerLine("驼峰", TextTransform.ToCamelCase);

    [RelayCommand]
    private void ToPascalCase() => ApplyPerLine("Pascal", TextTransform.ToPascalCase);

    [RelayCommand]
    private void ToSnakeCase() => ApplyPerLine("下划线", TextTransform.ToSnakeCase);

    [RelayCommand]
    private void ToKebabCase() => ApplyPerLine("短横线", TextTransform.ToKebabCase);

    [RelayCommand]
    private void ToConstantCase() => ApplyPerLine("常量", TextTransform.ToConstantCase);

    [RelayCommand]
    private void ToTitleCase() => ApplyPerLine("标题", TextTransform.ToTitleCase);

    [RelayCommand]
    private void ToUpperCase() => Apply("大写", value => value.ToUpperInvariant());

    [RelayCommand]
    private void ToLowerCase() => Apply("小写", value => value.ToLowerInvariant());

    [RelayCommand]
    private void SortLines() =>
        Apply("排序", value => TextTransform.SortLines(value, ignoreCase: IgnoreCase));

    [RelayCommand]
    private void RemoveDuplicates() =>
        Apply("去重", value => TextTransform.RemoveDuplicateLines(value, IgnoreCase, TrimBeforeDedupe));

    [RelayCommand]
    private void RemoveEmptyLines() => Apply("去空行", TextTransform.RemoveEmptyLines);

    [RelayCommand]
    private void TrimLines() => Apply("去首尾空格", TextTransform.TrimLines);

    [RelayCommand]
    private void AddLineNumbers() => Apply("加行号", value => TextTransform.AddLineNumbers(value));

    [RelayCommand]
    private void Swap()
    {
        (Input, Output) = (Output, Input);
        SetStatus("已交换输入与输出");
    }

    [RelayCommand]
    private void Clear()
    {
        Input = string.Empty;
        Output = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy() => TryCopy(Output);

    private void Apply(string label, Func<string, string> transform)
    {
        Output = transform(Input);
        SetStatus($"{label}处理完成 · {Output.Length:N0} 字符");
    }

    /// <summary>
    /// 命名风格转换按行执行,避免多行输入被合并成一行。
    /// </summary>
    private void ApplyPerLine(string label, Func<string, string> transform)
    {
        var newLine = Input.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var lines = Input.Replace("\r\n", "\n").Split('\n');

        Output = string.Join(newLine, lines.Select(transform));
        SetStatus($"{label}处理完成 · {lines.Length} 行");
    }
}

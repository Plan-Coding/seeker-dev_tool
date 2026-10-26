using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.TextEncodings;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.TextEncodings;

public partial class EncodingToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private int _sourceIndex = 1;

    [ObservableProperty]
    private int _targetIndex;

    [ObservableProperty]
    private string _input = "浣犲ソ";

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAsCommand))]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileInfo = string.Empty;

    [ObservableProperty]
    private bool _writeBom;

    public EncodingToolViewModel()
    {
        Convert();
    }

    public IReadOnlyList<EncodingOption> Encodings => TextEncodingConverter.Options;

    [RelayCommand]
    private void Convert()
    {
        var source = Encodings[SourceIndex];
        var target = Encodings[TargetIndex];

        if (!TextEncodingConverter.TryConvert(Input, source.Id, target.Id, out var result, out var error))
        {
            SetStatus(error ?? "编码转换失败", true);
            return;
        }

        Output = result;
        SetStatus($"{source.DisplayName} → {target.DisplayName} · {result.Length:N0} 字符");
    }

    [RelayCommand]
    private void Swap()
    {
        (SourceIndex, TargetIndex) = (TargetIndex, SourceIndex);
        Convert();
    }

    [RelayCommand]
    private void LoadSample()
    {
        FilePath = string.Empty;
        FileInfo = string.Empty;
        SourceIndex = 1;
        TargetIndex = 0;
        Input = "浣犲ソ";
        Convert();
        SetStatus("示例:GBK 乱码 → UTF-8");
    }

    [RelayCommand]
    private void ChooseFile()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择要转换的文件",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (!FileEncodingService.TryRead(dialog.FileName, 256 * 1024, out var result, out var error))
        {
            SetStatus(error ?? "读取文件失败", true);
            return;
        }

        FilePath = dialog.FileName;
        Input = result.Text;

        var detectedIndex = FindIndex(result.Detection.Id);

        if (detectedIndex >= 0)
        {
            SourceIndex = detectedIndex;
        }

        FileInfo = $"{Path.GetFileName(dialog.FileName)} · {FormatSize(result.FileSize)} · 检测:{result.Detection.DisplayName}"
            + (result.Truncated ? " · 预览已截断" : string.Empty);

        Convert();
        SetStatus($"{FileInfo} · 目标:{Encodings[TargetIndex].DisplayName}");
    }

    [RelayCommand(CanExecute = nameof(CanSaveAs))]
    private void SaveAs()
    {
        var source = Encodings[SourceIndex];
        var target = Encodings[TargetIndex];

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "转换并另存为",
            FileName = Path.GetFileName(FilePath),
            Filter = "所有文件|*.*"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (!FileEncodingService.TryConvertFile(
                FilePath,
                dialog.FileName,
                source.Id,
                target.Id,
                WriteBom,
                out var error))
        {
            SetStatus(error ?? "保存失败", true);
            return;
        }

        SetStatus($"已保存:{dialog.FileName} · {source.DisplayName} → {target.DisplayName}");
    }

    private bool CanSaveAs() => !string.IsNullOrWhiteSpace(FilePath);

    [RelayCommand]
    private void BatchConvert()
    {
        var source = Encodings[SourceIndex];
        var target = Encodings[TargetIndex];

        var filesDialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择要批量转换的文件",
            Multiselect = true,
            CheckFileExists = true
        };

        if (filesDialog.ShowDialog() != true)
        {
            return;
        }

        var folderDialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "选择输出目录"
        };

        if (folderDialog.ShowDialog() != true)
        {
            return;
        }

        var success = 0;
        var failed = 0;
        var skipped = 0;

        foreach (var file in filesDialog.FileNames)
        {
            var outputPath = Path.Combine(folderDialog.FolderName, Path.GetFileName(file));

            if (string.Equals(
                    Path.GetFullPath(file),
                    Path.GetFullPath(outputPath),
                    StringComparison.OrdinalIgnoreCase))
            {
                skipped++;
                continue;
            }

            if (FileEncodingService.TryConvertFile(file, outputPath, source.Id, target.Id, WriteBom, out _))
            {
                success++;
            }
            else
            {
                failed++;
            }
        }

        SetStatus(
            $"批量转换完成:成功 {success},失败 {failed}"
            + (skipped > 0 ? $",跳过 {skipped}(输出目录与源文件相同)" : string.Empty));
    }

    [RelayCommand]
    private void Clear()
    {
        Input = string.Empty;
        Output = string.Empty;
        FilePath = string.Empty;
        FileInfo = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy() => TryCopy(Output);

    private int FindIndex(string encodingId)
    {
        for (var i = 0; i < Encodings.Count; i++)
        {
            if (string.Equals(Encodings[i].Id, encodingId, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        var value = (double)bytes;
        var unit = 0;

        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.#} {units[unit]}";
    }
}

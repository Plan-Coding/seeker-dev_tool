using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seeker.DevToolbox.Core.Tools.Security;
using Seeker.DevToolbox.UI.ViewModels;

namespace Seeker.DevToolbox.Tools.Security;

public partial class HashToolViewModel : ToolViewModelBase
{
    [ObservableProperty]
    private int _algorithmIndex = 2;

    [ObservableProperty]
    private string _input = "Seeker DevToolbox";

    [ObservableProperty]
    private string _hmacKey = string.Empty;

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    public HashToolViewModel()
    {
        Compute();
    }

    public IReadOnlyList<string> Algorithms { get; } = ["MD5", "SHA-1", "SHA-256", "SHA-512"];

    private HashKind CurrentKind => AlgorithmIndex switch
    {
        0 => HashKind.Md5,
        1 => HashKind.Sha1,
        3 => HashKind.Sha512,
        _ => HashKind.Sha256
    };

    [RelayCommand]
    private void Compute()
    {
        RunSafely(() =>
        {
            var useHmac = !string.IsNullOrEmpty(HmacKey);
            Output = useHmac
                ? HashTool.ComputeHmac(Input, HmacKey, CurrentKind)
                : HashTool.Compute(Input, CurrentKind);

            SetStatus(useHmac
                ? $"HMAC-{Algorithms[AlgorithmIndex]} 计算完成"
                : $"{Algorithms[AlgorithmIndex]} 计算完成");
        });
    }

    [RelayCommand]
    private void ChooseFile()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择要计算哈希的文件",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            FilePath = dialog.FileName;
            ComputeFile();
        }
    }

    [RelayCommand]
    private void ComputeFile()
    {
        if (!File.Exists(FilePath))
        {
            SetStatus("请先选择文件", true);
            return;
        }

        RunSafely(() =>
        {
            Output = HashTool.ComputeFile(FilePath, CurrentKind);
            var info = new FileInfo(FilePath);
            SetStatus($"{info.Name} · {FormatSize(info.Length)} · {Algorithms[AlgorithmIndex]} 计算完成");
        });
    }

    [RelayCommand]
    private void Clear()
    {
        Input = string.Empty;
        HmacKey = string.Empty;
        FilePath = string.Empty;
        Output = string.Empty;
        SetStatus("已清空");
    }

    [RelayCommand]
    private void Copy() => TryCopy(Output);

    private static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
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

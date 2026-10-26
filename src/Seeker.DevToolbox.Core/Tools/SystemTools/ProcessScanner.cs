using System.Diagnostics;

namespace Seeker.DevToolbox.Core.Tools.SystemTools;

public sealed record ProcessEntry(
    int Pid,
    string Name,
    long WorkingSetBytes,
    DateTime? StartTime,
    string? FilePath);

public static class ProcessScanner
{
    public static IReadOnlyList<ProcessEntry> GetProcesses()
    {
        var entries = new List<ProcessEntry>();

        foreach (var process in Process.GetProcesses())
        {
            using (process)
            {
                try
                {
                    entries.Add(new ProcessEntry(
                        Pid: process.Id,
                        Name: process.ProcessName,
                        WorkingSetBytes: SafeWorkingSet(process),
                        StartTime: SafeStartTime(process),
                        FilePath: SafePath(process)));
                }
                catch (InvalidOperationException)
                {
                    // 进程在枚举过程中退出,忽略。
                }
            }
        }

        return entries
            .OrderByDescending(entry => entry.WorkingSetBytes)
            .ThenBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static long SafeWorkingSet(Process process)
    {
        try
        {
            return process.WorkingSet64;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return 0;
        }
    }

    private static DateTime? SafeStartTime(Process process)
    {
        try
        {
            return process.StartTime;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
        {
            return null;
        }
    }

    private static string? SafePath(Process process)
    {
        try
        {
            return process.MainModule?.FileName;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
        {
            return null;
        }
    }
}

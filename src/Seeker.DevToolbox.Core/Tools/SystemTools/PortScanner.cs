using System.Diagnostics;

namespace Seeker.DevToolbox.Core.Tools.SystemTools;

public sealed record PortEntry(
    string Protocol,
    string LocalAddress,
    int Port,
    string State,
    int Pid,
    string ProcessName);

public static class PortScanner
{
    public static IReadOnlyList<PortEntry> GetListeningTcpPorts()
    {
        var output = RunNetstat();
        return ParseNetstat(output, ResolveProcessName);
    }

    public static IReadOnlyList<PortEntry> ParseNetstat(
        string output,
        Func<int, string?> processNameResolver)
    {
        var results = new List<PortEntry>();

        foreach (var rawLine in output.Split('\n'))
        {
            var line = rawLine.Trim();

            if (!line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5
                || !parts[3].Equals("LISTENING", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var local = parts[1];
            var separator = local.LastIndexOf(':');

            if (separator <= 0
                || !int.TryParse(local[(separator + 1)..], out var port)
                || !int.TryParse(parts[4], out var pid))
            {
                continue;
            }

            var address = local[..separator].Trim('[', ']');
            var processName = processNameResolver(pid) ?? "?";

            results.Add(new PortEntry("TCP", address, port, parts[3], pid, processName));
        }

        return results
            .OrderBy(entry => entry.Port)
            .ThenBy(entry => entry.LocalAddress, StringComparer.Ordinal)
            .ToList();
    }

    private static string RunNetstat()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo("netstat", "-ano -p tcp")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process is null)
            {
                return string.Empty;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(5000);
            return output;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return string.Empty;
        }
    }

    private static string? ResolveProcessName(int pid)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}

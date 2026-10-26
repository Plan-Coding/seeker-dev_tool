using System.Diagnostics;
using Seeker.DevToolbox.Core.Tools.SystemTools;

namespace Seeker.DevToolbox.Core.Tests;

public class ProcessScannerTests
{
    [Fact]
    public void GetProcesses_ContainsCurrentProcess()
    {
        using var current = Process.GetCurrentProcess();

        var entries = ProcessScanner.GetProcesses();

        Assert.NotEmpty(entries);
        Assert.Contains(entries, entry => entry.Pid == current.Id);
    }

    [Fact]
    public void GetProcesses_IsSortedByMemoryDescending()
    {
        var entries = ProcessScanner.GetProcesses();

        for (var i = 1; i < entries.Count; i++)
        {
            Assert.True(entries[i - 1].WorkingSetBytes >= entries[i].WorkingSetBytes);
        }
    }
}

using Seeker.DevToolbox.Core.Tools.SystemTools;

namespace Seeker.DevToolbox.Core.Tests;

public class PortScannerTests
{
    [Fact]
    public void ParseNetstat_ReturnsOnlyListeningTcpRows()
    {
        const string output = """
              TCP    0.0.0.0:135            0.0.0.0:0              LISTENING       1234
              TCP    [::]:443               [::]:0                 LISTENING       5678
              TCP    127.0.0.1:5000         127.0.0.1:6000         ESTABLISHED     9999
              UDP    0.0.0.0:500             *:*                                    7777
            """;

        var entries = PortScanner.ParseNetstat(output, pid => pid switch
        {
            1234 => "svchost",
            5678 => "nginx",
            _ => null
        });

        Assert.Equal(2, entries.Count);
        Assert.Equal(135, entries[0].Port);
        Assert.Equal("0.0.0.0", entries[0].LocalAddress);
        Assert.Equal("svchost", entries[0].ProcessName);
        Assert.Equal(443, entries[1].Port);
        Assert.Equal("::", entries[1].LocalAddress);
        Assert.Equal("nginx", entries[1].ProcessName);
    }

    [Fact]
    public void ParseNetstat_UnknownPid_UsesQuestionMark()
    {
        const string output = "  TCP    0.0.0.0:8080    0.0.0.0:0    LISTENING    4242";

        var entries = PortScanner.ParseNetstat(output, _ => null);

        Assert.Single(entries);
        Assert.Equal("?", entries[0].ProcessName);
        Assert.Equal(4242, entries[0].Pid);
    }

    [Fact]
    public void ParseNetstat_IgnoresMalformedLines()
    {
        const string output = """
            TCP
            TCP  not-a-port  0.0.0.0:0  LISTENING  1
            TCP 0.0.0.0:80 0.0.0.0:0 LISTENING 1
            """;

        Assert.Single(PortScanner.ParseNetstat(output, _ => "test"));
    }
}

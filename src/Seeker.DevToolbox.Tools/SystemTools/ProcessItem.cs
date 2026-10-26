namespace Seeker.DevToolbox.Tools.SystemTools;

public sealed record ProcessItem(
    int Pid,
    string Name,
    string Memory,
    string StartTime,
    string FilePath);

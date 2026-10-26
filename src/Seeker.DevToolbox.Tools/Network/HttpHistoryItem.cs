namespace Seeker.DevToolbox.Tools.Network;

public sealed record HttpHistoryItem(
    string Method,
    string Url,
    string Headers,
    string Body,
    string Cookie,
    string Display);

namespace Seeker.DevToolbox.Core.Configuration;

/// <summary>
/// 持久化到 data/settings.json 的用户设置。字段保持简单,方便手改和迁移。
/// </summary>
public sealed class AppSettings
{
    public int SchemaVersion { get; set; } = 1;

    /// <summary>System / Dark / Light</summary>
    public string Theme { get; set; } = "System";

    public string? LastToolId { get; set; }

    public List<string> RecentToolIds { get; set; } = [];

    public double WindowWidth { get; set; } = 1100;

    public double WindowHeight { get; set; } = 720;

    public bool WindowMaximized { get; set; }
}

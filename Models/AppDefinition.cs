namespace MacOSBlazor.Models;

/// <summary>App entry shown in Dock and Launchpad (mirrors .taskbarApp / .appMenu .app).</summary>
public sealed record AppDefinition(
    string Id,
    string Name,
    string Icon,
    string? WindowTitle = null,
    int DefaultWidth = 720,
    int DefaultHeight = 480,
    int DefaultLeft = 220,
    int DefaultTop = 90);

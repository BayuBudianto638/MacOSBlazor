namespace MacOSBlazor.Models;

/// <summary>Runtime state of one open window (mirrors .window + JS zIndex logic).</summary>
public sealed class DesktopWindow
{
    private static int _seq;

    public DesktopWindow(string appId, string title, int width, int height, int left, int top)
    {
        InstanceId = $"w{Interlocked.Increment(ref _seq)}";
        AppId = appId;
        Title = title;
        Width = width;
        Height = height;
        Left = left;
        Top = top;
    }

    public string InstanceId { get; }
    public string AppId { get; }
    public string Title { get; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }
    public int ZIndex { get; set; }
    public bool Minimized { get; set; }
    public bool Maximized { get; set; }
    public bool IsActive { get; set; }
}

using MacOSBlazor.Models;

namespace MacOSBlazor.Services;

/// <summary>
/// Window manager: open/focus/minimize/close/maximize + z-order.
/// C# equivalent of the sWindowUI() logic in script.js.
/// </summary>
public sealed class WindowManager
{
    private int _zTop;
    public event Action? Changed;

    public List<AppDefinition> Apps { get; } = new()
    {
        new("finder", "Finder", "📁", "Finder", 760, 480, 180, 110),
        new("safari", "Safari", "🧭", "Safari", 1020, 640, 260, 60),
        new("messages", "Messages", "💬", "Messages", 760, 500, 300, 100),
        new("calendar", "Calendar", "📅", "Calendar", 720, 480, 340, 130),
        new("preferences", "System Preferences", "⚙️", "System Preferences", 700, 500, 380, 110),
        new("terminal", "Terminal", "💻", "Terminal — zsh", 620, 400, 420, 150),
    };

    public List<DesktopWindow> Windows { get; } = new();

    public string? ActiveAppId { get; private set; } = "finder";
    public bool LaunchpadOpen { get; private set; }
    public bool NotificationCenterOpen { get; private set; }
    public string? OpenMenuPanel { get; private set; }

    public bool IsAppOpen(string appId) => Windows.Exists(w => w.AppId == appId && !w.Minimized)
        || Windows.Exists(w => w.AppId == appId);

    public bool HasAnyVisible(string appId) => Windows.Exists(w => w.AppId == appId && !w.Minimized);

    public void Open(string appId)
    {
        var def = Apps.Find(a => a.Id == appId);
        if (def is null) return;

        var existing = Windows.Find(w => w.AppId == appId);
        if (existing is not null)
        {
            existing.Minimized = false;
            Focus(existing.InstanceId);
            return;
        }

        var cascade = Windows.Count * 28;
        var win = new DesktopWindow(
            def.Id, def.WindowTitle ?? def.Name,
            def.DefaultWidth, def.DefaultHeight,
            def.DefaultLeft + cascade, def.DefaultTop + cascade);
        Windows.Add(win);
        Focus(win.InstanceId);
        LaunchpadOpen = false;
    }

    public void Focus(string instanceId)
    {
        var win = Windows.Find(w => w.InstanceId == instanceId);
        if (win is null) return;
        win.ZIndex = ++_zTop;
        win.Minimized = false;
        foreach (var w in Windows) w.IsActive = w.InstanceId == instanceId;
        ActiveAppId = win.AppId;
        Notify();
    }

    public void Minimize(string instanceId)
    {
        var win = Windows.Find(w => w.InstanceId == instanceId);
        if (win is null) return;
        win.Minimized = true;
        win.IsActive = false;
        var top = TopVisible();
        ActiveAppId = top?.AppId;
        if (top is not null) foreach (var w in Windows) w.IsActive = w == top;
        Notify();
    }

    public void Close(string instanceId)
    {
        var removed = Windows.RemoveAll(w => w.InstanceId == instanceId) > 0;
        if (!removed) return;
        var top = TopVisible();
        ActiveAppId = top?.AppId ?? "finder";
        foreach (var w in Windows) w.IsActive = w == top;
        Notify();
    }

    public void ToggleMaximize(string instanceId)
    {
        var win = Windows.Find(w => w.InstanceId == instanceId);
        if (win is null) return;
        win.Maximized = !win.Maximized;
        Focus(instanceId);
    }

    public void Move(string instanceId, int left, int top)
    {
        var win = Windows.Find(w => w.InstanceId == instanceId);
        if (win is null) return;
        win.Left = Math.Max(-win.Width + 80, left);
        win.Top = Math.Clamp(top, 22, 2000);
        Notify();
    }

    public void SetLaunchpad(bool open) { LaunchpadOpen = open; Notify(); }
    public void SetNotificationCenter(bool open) { NotificationCenterOpen = open; Notify(); }
    public void SetMenuPanel(string? panel) { OpenMenuPanel = panel; Notify(); }
    public void CloseAllPanels() { OpenMenuPanel = null; NotificationCenterOpen = false; Notify(); }

    public DesktopWindow? ActiveWindow =>
        Windows.Find(w => w.IsActive) ?? TopVisible();

    public void MinimizeActive()
    {
        var w = ActiveWindow;
        if (w is not null) Minimize(w.InstanceId);
    }

    public void CloseActive()
    {
        var w = ActiveWindow;
        if (w is not null) Close(w.InstanceId);
    }

    public void ToggleMaximizeActive()
    {
        var w = ActiveWindow;
        if (w is not null) ToggleMaximize(w.InstanceId);
    }

    public void BringAllToFront()
    {
        if (ActiveAppId is null) return;
        DesktopWindow? top = null;
        foreach (var w in Windows)
            if (w.AppId == ActiveAppId)
            {
                w.Minimized = false;
                if (top is null || w.ZIndex > top.ZIndex) top = w;
            }
        if (top is not null) Focus(top.InstanceId);
        else Notify();
    }

    public void QuitApp()
    {
        if (ActiveAppId is null) return;
        Windows.RemoveAll(w => w.AppId == ActiveAppId);
        var top = TopVisible();
        ActiveAppId = top?.AppId ?? "finder";
        foreach (var w in Windows) w.IsActive = w == top;
        Notify();
    }

    private DesktopWindow? TopVisible()
    {
        DesktopWindow? top = null;
        foreach (var w in Windows)
            if (!w.Minimized && (top is null || w.ZIndex > top.ZIndex))
                top = w;
        return top;
    }

    private void Notify() => Changed?.Invoke();
}

namespace MacOSBlazor.Services;

/// <summary>
/// 1-second ticker shared by MenuBar clock, Notification Center and wallpaper.
/// Mirrors renderTime() in the original script.js.
/// </summary>
public sealed class MacClock : IDisposable
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
    private readonly CancellationTokenSource _cts = new();

    public event Action? Tick;
    public DateTime Now { get; private set; } = DateTime.Now;

    public MacClock()
    {
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                Now = DateTime.Now;
                Tick?.Invoke();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>Menu bar text, e.g. "Sen 19 Okt 21:30" or "21:30:05".</summary>
    public string MenuBarText(MacSettings settings)
    {
        var t = Now.ToString(settings.ShowSecondsInMenuBar ? "HH:mm:ss" : "HH:mm");
        if (!settings.ShowDateInMenuBar)
            return t;
        return $"{Now:ddd} {Now:dd} {Now:MMM} {t}";
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer.Dispose();
        _cts.Dispose();
    }
}

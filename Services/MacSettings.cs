namespace MacOSBlazor.Services;

/// <summary>
/// Toggleable macOS system settings, mirrored from the original
/// System Preferences checkboxes (sDynamicWallpaper, sTimeShowDate, ...).
/// </summary>
public sealed class MacSettings
{
    public event Action? Changed;

    public bool AnimateDockIconOnOpen { get; private set; } = true;
    public bool DynamicWallpaper { get; private set; } = true;
    public bool ForceNight { get; private set; }
    public bool ShowDateInMenuBar { get; private set; } = true;
    public bool ShowSecondsInMenuBar { get; private set; }
    public bool ShowBatteryPercentage { get; private set; } = true;
    public int BatteryPercentage { get; private set; } = 74;
    public bool ReduceTransparency { get; private set; }

    public bool IsNight(DateTime now) =>
        ForceNight || !DynamicWallpaper || now.Hour < 7 || now.Hour >= 19;

    public void ToggleAnimateDockIconOnOpen() { AnimateDockIconOnOpen = !AnimateDockIconOnOpen; Notify(); }
    public void ToggleDynamicWallpaper() { DynamicWallpaper = !DynamicWallpaper; Notify(); }
    public void ToggleForceNight() { ForceNight = !ForceNight; Notify(); }
    public void ToggleShowDate() { ShowDateInMenuBar = !ShowDateInMenuBar; Notify(); }
    public void ToggleShowSeconds() { ShowSecondsInMenuBar = !ShowSecondsInMenuBar; Notify(); }
    public void ToggleShowBatteryPercentage() { ShowBatteryPercentage = !ShowBatteryPercentage; Notify(); }
    public void ToggleReduceTransparency() { ReduceTransparency = !ReduceTransparency; Notify(); }

    private void Notify() => Changed?.Invoke();
}

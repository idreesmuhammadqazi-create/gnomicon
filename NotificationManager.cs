namespace Gnomicon;

/// <summary>
/// Manages toast notifications for the application.
/// </summary>
public class NotificationManager
{
    private readonly NotifyIcon _notifyIcon;

    public NotificationManager(NotifyIcon notifyIcon)
    {
        _notifyIcon = notifyIcon;
    }

    /// <summary>
    /// Shows a notification that icons have been rearranged.
    /// </summary>
    public void ShowRearrangementNotification(RearrangementMode mode)
    {
        string title = "Gnomicon";
        string message = mode switch
        {
            RearrangementMode.FullChaos => "Gnomicon has improved your feng shui with full chaos!",
            RearrangementMode.Sneaky => "Gnomicon has subtly improved your feng shui.",
            RearrangementMode.Orbit => "Gnomicon has aligned your icons with the cosmos.",
            _ => "Gnomicon has improved your feng shui."
        };

        ShowNotification(title, message, ToolTipIcon.Info);
    }

    /// <summary>
    /// Shows a notification that the application has started.
    /// </summary>
    public void ShowStartupNotification()
    {
        ShowNotification(
            "Gnomicon",
            "Gnomicon is now running in the system tray. Your desktop icons are being watched...",
            ToolTipIcon.Info);
    }

    /// <summary>
    /// Shows a notification that original positions have been restored.
    /// </summary>
    public void ShowRestoreNotification()
    {
        ShowNotification(
            "Gnomicon",
            "Your desktop icons have been restored to their original positions.",
            ToolTipIcon.Info);
    }

    /// <summary>
    /// Shows a notification that the application is paused.
    /// </summary>
    public void ShowPauseNotification(TimeSpan duration)
    {
        ShowNotification(
            "Gnomicon",
            $"Gnomicon is paused for {duration.TotalHours:F0} hour(s). Your icons are safe... for now.",
            ToolTipIcon.Warning);
    }

    /// <summary>
    /// Shows a notification that the application has been resumed.
    /// </summary>
    public void ShowResumeNotification()
    {
        ShowNotification(
            "Gnomicon",
            "Gnomicon is active again! Watch your desktop...",
            ToolTipIcon.Info);
    }

    /// <summary>
    /// Shows a generic notification.
    /// </summary>
    private void ShowNotification(string title, string message, ToolTipIcon icon)
    {
        if (_notifyIcon == null)
            return;

        try
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }
        catch
        {
            // Ignore notification errors
        }
    }
}

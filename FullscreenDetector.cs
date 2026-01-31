using System.Runtime.InteropServices;

namespace Gnomicon;

/// <summary>
/// Detects if a fullscreen application is currently running.
/// </summary>
public class FullscreenDetector
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("shell32.dll")]
    private static extern int SHQueryUserNotificationState(out QUERY_USER_NOTIFICATION_STATE pquns);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private enum QUERY_USER_NOTIFICATION_STATE
    {
        QUNS_NOT_PRESENT = 1,
        QUNS_BUSY = 2,
        QUNS_RUNNING_D3D_FULL_SCREEN = 3,
        QUNS_PRESENTATION_MODE = 4,
        QUNS_ACCEPTS_NOTIFICATIONS = 5,
        QUNS_QUIET_TIME = 6,
        QUNS_APP = 7
    }

    public bool IsFullscreenAppRunning()
    {
        try
        {
            int result = SHQueryUserNotificationState(out QUERY_USER_NOTIFICATION_STATE state);
            if (result == 0)
            {
                if (state == QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN ||
                    state == QUERY_USER_NOTIFICATION_STATE.QUNS_BUSY ||
                    state == QUERY_USER_NOTIFICATION_STATE.QUNS_PRESENTATION_MODE)
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        return IsForegroundWindowFullscreen();
    }

    private bool IsForegroundWindowFullscreen()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        
        if (foregroundWindow == IntPtr.Zero)
            return false;

        if (!IsWindow(foregroundWindow) || !IsWindowVisible(foregroundWindow))
            return false;

        if (!GetWindowRect(foregroundWindow, out RECT windowRect))
            return false;

        var primaryScreen = Screen.PrimaryScreen;
        if (primaryScreen == null)
            return false;

        var screenBounds = primaryScreen.Bounds;

        int windowWidth = windowRect.Right - windowRect.Left;
        int windowHeight = windowRect.Bottom - windowRect.Top;

        const int tolerance = 10;
        bool coversScreen = 
            Math.Abs(windowRect.Left - screenBounds.Left) <= tolerance &&
            Math.Abs(windowRect.Top - screenBounds.Top) <= tolerance &&
            Math.Abs(windowWidth - screenBounds.Width) <= tolerance &&
            Math.Abs(windowHeight - screenBounds.Height) <= tolerance;

        return coversScreen;
    }

    public string GetFullscreenStateInfo()
    {
        try
        {
            int result = SHQueryUserNotificationState(out QUERY_USER_NOTIFICATION_STATE state);
            return $"SHQueryUserNotificationState: {state} (HRESULT: {result})";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}

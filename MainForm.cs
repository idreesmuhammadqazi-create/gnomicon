namespace Gnomicon;

/// <summary>
/// Main application form. Hidden form that hosts the system tray icon.
/// </summary>
public class MainForm : Form
{
    private readonly DesktopIconManager _iconManager;
    private readonly PositionValidator _positionValidator;
    private readonly RearrangementEngine _rearrangementEngine;
    private readonly FullscreenDetector _fullscreenDetector;
    private readonly SettingsManager _settingsManager;
    private NotificationManager? _notificationManager;

    private NotifyIcon? _trayIcon;
    private ContextMenuStrip? _trayMenu;
    private System.Windows.Forms.Timer? _rearrangementTimer;
    private System.Windows.Forms.Timer? _pauseTimer;

    private List<IconPosition> _currentPositions = new();
    private bool _isInitialized = false;

    public MainForm()
    {
        try
        {
            _iconManager = new DesktopIconManager();
            _positionValidator = new PositionValidator();
            _rearrangementEngine = new RearrangementEngine(_positionValidator);
            _fullscreenDetector = new FullscreenDetector();
            _settingsManager = new SettingsManager();

            Text = "Gnomicon";
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(0, 0);
            Opacity = 0;
            Show();
            Hide();

            if (!_iconManager.Initialize())
            {
                MessageBox.Show(
                    "Failed to initialize desktop icon manager.\n\n" +
                    "This may be due to:\n" +
                    "- Windows version incompatibility\n" +
                    "- Insufficient permissions\n" +
                    "- Desktop not available\n\n" +
                    "The application will run but icon rearrangement will be disabled.",
                    "Gnomicon Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                
                _settingsManager.Settings.IsEnabled = false;
            }
            else
            {
                _isInitialized = true;
            }

            _settingsManager.Load();
            InitializeTrayIcon();
            InitializeTimers();

            if (_isInitialized && !_settingsManager.Settings.HasOriginalPositions)
            {
                SaveCurrentPositionsAsOriginal();
            }

            if (_settingsManager.Settings.ShowNotifications)
            {
                _notificationManager?.ShowStartupNotification();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Gnomicon encountered an error during startup:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Gnomicon Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void InitializeTrayIcon()
    {
        try
        {
            _trayMenu = new ContextMenuStrip();
            
            var statusLabel = new ToolStripMenuItem("Gnomicon - Active")
            {
                Enabled = false,
                Font = new Font(Font, FontStyle.Bold)
            };
            _trayMenu.Items.Add(statusLabel);
            _trayMenu.Items.Add(new ToolStripSeparator());

            var toggleItem = new ToolStripMenuItem("Enabled", null, OnToggleEnabled)
            {
                Checked = _settingsManager.Settings.IsEnabled && !_settingsManager.Settings.IsPaused
            };
            _trayMenu.Items.Add(toggleItem);

            var pauseItem = new ToolStripMenuItem("Pause for 1 hour", null, OnPauseOneHour);
            _trayMenu.Items.Add(pauseItem);

            var intervalItem = new ToolStripMenuItem($"Set Interval ({_settingsManager.Settings.IntervalMinutes} min)", null, OnSetInterval);
            _trayMenu.Items.Add(intervalItem);

            _trayMenu.Items.Add(new ToolStripSeparator());

            var modeMenu = new ToolStripMenuItem("Mode");
            foreach (RearrangementMode mode in Enum.GetValues(typeof(RearrangementMode)))
            {
                var modeItem = new ToolStripMenuItem(mode.GetDisplayName(), null, OnModeSelected)
                {
                    Tag = mode,
                    Checked = _settingsManager.Settings.Mode == mode
                };
                modeMenu.DropDownItems.Add(modeItem);
            }
            _trayMenu.Items.Add(modeMenu);

            _trayMenu.Items.Add(new ToolStripSeparator());

            var restoreItem = new ToolStripMenuItem("Restore Original Layout", null, OnRestoreLayout)
            {
                Enabled = _settingsManager.Settings.HasOriginalPositions
            };
            _trayMenu.Items.Add(restoreItem);

            var saveItem = new ToolStripMenuItem("Save Current as Original", null, OnSaveCurrentLayout);
            _trayMenu.Items.Add(saveItem);

            _trayMenu.Items.Add(new ToolStripSeparator());

            var notifyItem = new ToolStripMenuItem("Show Notifications", null, OnToggleNotifications)
            {
                Checked = _settingsManager.Settings.ShowNotifications
            };
            _trayMenu.Items.Add(notifyItem);

            _trayMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem("Exit", null, OnExit);
            _trayMenu.Items.Add(exitItem);

            _trayIcon = new NotifyIcon
            {
                Text = "Gnomicon - Desktop Icon Rearranger",
                Icon = SystemIcons.Application,
                ContextMenuStrip = _trayMenu,
                Visible = true
            };

            _trayIcon.DoubleClick += OnTrayDoubleClick;
            _notificationManager = new NotificationManager(_trayIcon);
            UpdateMenuState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing tray icon: {ex.Message}");
        }
    }

    private void InitializeTimers()
    {
        try
        {
            _rearrangementTimer = new System.Windows.Forms.Timer
            {
                Interval = _settingsManager.Settings.IntervalMinutes * 60 * 1000
            };
            _rearrangementTimer.Tick += OnRearrangementTimerTick;

            if (_settingsManager.Settings.IsEnabled && !_settingsManager.Settings.IsPaused && _isInitialized)
            {
                _rearrangementTimer.Start();
            }

            _pauseTimer = new System.Windows.Forms.Timer
            {
                Interval = 60000
            };
            _pauseTimer.Tick += OnPauseTimerTick;
            _pauseTimer.Start();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing timers: {ex.Message}");
        }
    }

    private void UpdateMenuState()
    {
        if (_trayMenu == null) return;

        try
        {
            var statusLabel = (ToolStripMenuItem)_trayMenu.Items[0];
            if (!_isInitialized)
            {
                statusLabel.Text = "Gnomicon - Not Initialized";
            }
            else if (_settingsManager.Settings.IsPaused)
            {
                statusLabel.Text = $"Gnomicon - Paused ({_settingsManager.Settings.PauseRemainingText} remaining)";
            }
            else if (_settingsManager.Settings.IsEnabled)
            {
                statusLabel.Text = $"Gnomicon - Active ({_settingsManager.Settings.Mode.GetDisplayName()})";
            }
            else
            {
                statusLabel.Text = "Gnomicon - Disabled";
            }

            var toggleItem = (ToolStripMenuItem)_trayMenu.Items[2];
            toggleItem.Checked = _settingsManager.Settings.IsEnabled && !_settingsManager.Settings.IsPaused;
            toggleItem.Text = _settingsManager.Settings.IsEnabled ? "Enabled" : "Disabled";

            var modeMenu = (ToolStripMenuItem)_trayMenu.Items[5];
            foreach (ToolStripMenuItem modeItem in modeMenu.DropDownItems)
            {
                if (modeItem.Tag is RearrangementMode mode)
                {
                    modeItem.Checked = _settingsManager.Settings.Mode == mode;
                }
            }

            var restoreItem = (ToolStripMenuItem)_trayMenu.Items[7];
            restoreItem.Enabled = _settingsManager.Settings.HasOriginalPositions;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating menu state: {ex.Message}");
        }
    }

    private void SaveCurrentPositionsAsOriginal()
    {
        try
        {
            var positions = _iconManager.GetAllIconPositions();
            _settingsManager.Settings.SaveOriginalPositions(positions);
            _settingsManager.Save();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving original positions: {ex.Message}");
        }
    }

    private void PerformRearrangement()
    {
        try
        {
            if (!_settingsManager.Settings.IsEnabled || _settingsManager.Settings.IsPaused || !_isInitialized)
                return;

            if (_fullscreenDetector.IsFullscreenAppRunning())
                return;

            var currentPositions = _iconManager.GetAllIconPositions();
            if (currentPositions.Count == 0)
                return;

            var newPositions = _rearrangementEngine.Rearrange(
                currentPositions, 
                _settingsManager.Settings.Mode);

            _iconManager.SetIconPositions(newPositions);

            if (_settingsManager.Settings.ShowNotifications)
            {
                _notificationManager?.ShowRearrangementNotification(_settingsManager.Settings.Mode);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during rearrangement: {ex.Message}");
        }
    }

    #region Event Handlers

    private void OnToggleEnabled(object? sender, EventArgs e)
    {
        try
        {
            _settingsManager.Settings.IsEnabled = !_settingsManager.Settings.IsEnabled;
            
            if (_settingsManager.Settings.IsEnabled && _isInitialized)
            {
                _rearrangementTimer?.Start();
            }
            else
            {
                _rearrangementTimer?.Stop();
            }
            
            _settingsManager.Save();
            UpdateMenuState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling enabled: {ex.Message}");
        }
    }

    private void OnPauseOneHour(object? sender, EventArgs e)
    {
        try
        {
            _settingsManager.Settings.PauseFor(TimeSpan.FromHours(1));
            _rearrangementTimer?.Stop();
            _settingsManager.Save();
            
            if (_settingsManager.Settings.ShowNotifications)
            {
                _notificationManager?.ShowPauseNotification(TimeSpan.FromHours(1));
            }
            
            UpdateMenuState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error pausing: {ex.Message}");
        }
    }

    private void OnSetInterval(object? sender, EventArgs e)
    {
        try
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the interval in minutes between icon rearrangements:",
                "Set Interval",
                _settingsManager.Settings.IntervalMinutes.ToString());
            
            if (int.TryParse(input, out int newInterval) && newInterval >= 1 && newInterval <= 1440)
            {
                _settingsManager.Settings.IntervalMinutes = newInterval;
                _settingsManager.Save();
                
                if (_rearrangementTimer != null)
                {
                    bool wasRunning = _rearrangementTimer.Enabled;
                    _rearrangementTimer.Stop();
                    _rearrangementTimer.Interval = newInterval * 60 * 1000;
                    if (wasRunning && _settingsManager.Settings.IsEnabled && !_settingsManager.Settings.IsPaused && _isInitialized)
                    {
                        _rearrangementTimer.Start();
                    }
                }
                
                UpdateMenuState();
                
                MessageBox.Show(
                    $"Interval set to {newInterval} minutes.",
                    "Gnomicon",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (!string.IsNullOrEmpty(input))
            {
                MessageBox.Show(
                    "Please enter a valid number between 1 and 1440 minutes.",
                    "Gnomicon",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting interval: {ex.Message}");
        }
    }

    private void OnModeSelected(object? sender, EventArgs e)
    {
        try
        {
            if (sender is ToolStripMenuItem item && item.Tag is RearrangementMode mode)
            {
                _settingsManager.Settings.Mode = mode;
                _settingsManager.Save();
                UpdateMenuState();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selecting mode: {ex.Message}");
        }
    }

    private void OnRestoreLayout(object? sender, EventArgs e)
    {
        try
        {
            if (!_settingsManager.Settings.HasOriginalPositions)
                return;

            _iconManager.SetIconPositions(_settingsManager.Settings.OriginalPositions);
            
            if (_settingsManager.Settings.ShowNotifications)
            {
                _notificationManager?.ShowRestoreNotification();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error restoring layout: {ex.Message}");
        }
    }

    private void OnSaveCurrentLayout(object? sender, EventArgs e)
    {
        try
        {
            SaveCurrentPositionsAsOriginal();
            
            MessageBox.Show(
                "Current desktop icon positions have been saved as the original layout.",
                "Gnomicon",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            
            UpdateMenuState();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to save current layout: {ex.Message}",
                "Gnomicon Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void OnToggleNotifications(object? sender, EventArgs e)
    {
        try
        {
            _settingsManager.Settings.ShowNotifications = !_settingsManager.Settings.ShowNotifications;
            _settingsManager.Save();
            
            if (sender is ToolStripMenuItem item)
            {
                item.Checked = _settingsManager.Settings.ShowNotifications;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling notifications: {ex.Message}");
        }
    }

    private void OnExit(object? sender, EventArgs e)
    {
        try
        {
            _trayIcon?.Dispose();
            Application.Exit();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exiting: {ex.Message}");
        }
    }

    private void OnTrayDoubleClick(object? sender, EventArgs e)
    {
        OnToggleEnabled(sender, e);
    }

    private void OnRearrangementTimerTick(object? sender, EventArgs e)
    {
        PerformRearrangement();
    }

    private void OnPauseTimerTick(object? sender, EventArgs e)
    {
        try
        {
            if (_settingsManager.Settings.IsPaused)
            {
                UpdateMenuState();
            }
            else if (_settingsManager.Settings.PauseUntil == null && 
                     _settingsManager.Settings.IsEnabled &&
                     _rearrangementTimer != null &&
                     !_rearrangementTimer.Enabled &&
                     _isInitialized)
            {
                _rearrangementTimer.Start();
                
                if (_settingsManager.Settings.ShowNotifications)
                {
                    _notificationManager?.ShowResumeNotification();
                }
                
                UpdateMenuState();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in pause timer tick: {ex.Message}");
        }
    }

    #endregion

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _rearrangementTimer?.Stop();
        _rearrangementTimer?.Dispose();
        _pauseTimer?.Stop();
        _pauseTimer?.Dispose();
        _trayIcon?.Dispose();
        base.OnFormClosing(e);
    }
}

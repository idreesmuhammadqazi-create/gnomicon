# Gnomicon

A lightweight Windows background application that playfully rearranges desktop icons every 60 minutes.

## Features

- **System Tray Application**: Lives quietly in the system tray
- **Three Rearrangement Modes**:
  - **Full Chaos**: Randomizes all icon positions
  - **Sneaky**: Subtly swaps only 2-4 icon positions
  - **Orbit**: Arranges icons in a circular pattern around the screen center
- **Safety Features**:
  - Never places icons off-screen
  - Detects fullscreen applications and pauses automatically
  - Saves and restores original icon positions
- **Tray Menu Options**:
  - Enable/disable rearrangement
  - Pause for 1 hour
  - Restore original layout
  - Change rearrangement mode
  - Toggle notifications
- **Optional Notifications**: "Gnomicon has improved your feng shui."
- **Minimal CPU Usage**: Timer-based operation (no constant polling)

## How Icon Positioning Works

### Windows Desktop Architecture

The Windows desktop is actually a **ListView control** (class name: `SysListView32`) managed by the Windows shell. Here's how Gnomicon interacts with it:

1. **Finding the Desktop Window**:
   ```
   Progman (Program Manager)
   └── SHELLDLL_DefView
       └── SysListView32 (Desktop icons)
   ```
   
   On Windows 10/11, the desktop may also be under a `WorkerW` window.

2. **Windows API Messages**:
   - `LVM_GETITEMCOUNT` (0x1004): Gets the number of desktop icons
   - `LVM_GETITEMPOSITION` (0x1010): Reads an icon's x,y coordinates
   - `LVM_SETITEMPOSITION` (0x100F): Sets an icon's position

3. **Cross-Process Memory Access**:
   Since the desktop runs in a different process (explorer.exe), Gnomicon uses `VirtualAllocEx` and `ReadProcessMemory`/`WriteProcessMemory` to access the icon position data.

### Rearrangement Process

1. **Read Current Positions**: Query all desktop icon positions via Windows API
2. **Apply Transformation**: Based on selected mode:
   - Full Chaos: Generate random positions within screen bounds
   - Sneaky: Swap positions between 2-4 randomly selected icons
   - Orbit: Calculate circular positions around screen center
3. **Validate Positions**: Ensure all icons stay within visible screen area
4. **Apply New Positions**: Send updated positions back to the desktop ListView

### Safety Mechanisms

- **Screen Bounds**: Uses `Screen.WorkingArea` to respect taskbar boundaries
- **Fullscreen Detection**: Uses `SHQueryUserNotificationState` API to detect when games or videos are running fullscreen
- **Overlap Prevention**: Minimum 60-pixel spacing between icons
- **Position Persistence**: Original positions saved to `%LOCALAPPDATA%\Gnomicon\settings.json`

## Build Instructions

### Prerequisites

- .NET 8.0 SDK or later
- Windows 10 or Windows 11

### Build Steps

1. **Clone or extract the source code**:
   ```cmd
   cd c:\Users\Kids\Documents\GitHub\gnomicon
   ```

2. **Build the project**:
   ```cmd
   dotnet build
   ```

3. **Run the application**:
   ```cmd
   dotnet run
   ```

   Or run the compiled executable:
   ```cmd
   dotnet build -c Release
   .\bin\Release\net8.0-windows\Gnomicon.exe
   ```

### Publishing as Single File

To create a standalone executable:

```cmd
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

The executable will be in:
```
.\bin\Release\net8.0-windows\win-x64\publish\Gnomicon.exe
```

## Usage

### First Run

1. Run `Gnomicon.exe`
2. The application will:
   - Save your current desktop layout as the "original" positions
   - Show a system tray icon
   - Start with the "Sneaky" mode enabled

### System Tray Menu

Right-click the tray icon to access:

- **Enabled/Disabled**: Toggle rearrangement on/off
- **Pause for 1 hour**: Temporarily disable for 1 hour
- **Set Interval**: Set the time interval between rearrangements (1-1440 minutes)
- **Mode**: Select rearrangement mode (Full Chaos, Sneaky, Orbit)
- **Restore Original Layout**: Return icons to their saved original positions
- **Save Current as Original**: Update the saved "original" layout
- **Show Notifications**: Toggle notification balloons
- **Exit**: Close the application

### Double-Click

Double-click the tray icon to quickly toggle enabled/disabled state.

## Configuration

Settings are stored in:
```
%LOCALAPPDATA%\Gnomicon\settings.json
```

Example configuration:
```json
{
  "isEnabled": true,
  "mode": "Sneaky",
  "pauseUntil": null,
  "showNotifications": true,
  "intervalMinutes": 60,
  "originalPositions": [
    {"index": 0, "x": 100, "y": 100, "name": null},
    {"index": 1, "x": 100, "y": 164, "name": null}
  ]
}
```

**Note**: `intervalMinutes` can be set between 1 and 1440 minutes (1 minute to 24 hours).

## Technical Details

### Project Structure

```
Gnomicon/
├── AppSettings.cs              # Configuration model
├── DesktopIconManager.cs       # Windows API interop for icon manipulation
├── FullscreenDetector.cs       # Fullscreen application detection
├── IconPosition.cs             # Icon position data model
├── MainForm.cs                 # Main application form (hidden, hosts tray)
├── NotificationManager.cs      # Toast notification handling
├── PositionValidator.cs        # Screen bounds and overlap validation
├── Program.cs                  # Application entry point
├── RearrangementEngine.cs      # Rearrangement mode implementations
├── RearrangementMode.cs        # Mode enum and extensions
├── SettingsManager.cs          # Configuration persistence
└── Gnomicon.csproj             # Project file
```

### Timer Strategy

- **Main Timer**: 60-minute interval (configurable)
- **Pause Timer**: 1-minute interval to check for pause expiration
- **No Polling**: Uses `System.Windows.Forms.Timer` for minimal CPU usage
- **Smart Scheduling**: Checks fullscreen state before rearranging

### Single Instance

Uses a named mutex (`Gnomicon_SingleInstance`) to ensure only one instance runs at a time.

## Troubleshooting

### "Failed to initialize desktop icon manager"

This error occurs when Gnomicon cannot access the desktop ListView. Possible causes:

1. **Windows Version**: The desktop window hierarchy may differ on some Windows versions
2. **Permissions**: Try running as administrator
3. **Desktop Not Available**: Ensure Windows Explorer is running

### Icons Not Moving

1. Check if a fullscreen application is running (Gnomicon auto-pauses)
2. Verify the application is enabled in the tray menu
3. Check if pause is active

### Notifications Not Showing

Windows may suppress notifications. Check:
- Windows notification settings
- "Show Notifications" is enabled in Gnomicon menu
- Focus Assist is not active

## License

This is a sample application for educational purposes.

## Credits

Built with C# and Windows Forms, using P/Invoke for Windows API access.

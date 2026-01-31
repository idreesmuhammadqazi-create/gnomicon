using System.Text.Json;

namespace Gnomicon;

/// <summary>
/// Manages loading and saving of application settings.
/// </summary>
public class SettingsManager
{
    private readonly string _settingsDirectory;
    private readonly string _settingsFilePath;
    private readonly object _lockObject = new();

    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    public AppSettings Settings { get; private set; }

    /// <summary>
    /// Event raised when settings are saved.
    /// </summary>
    public event EventHandler? SettingsSaved;

    /// <summary>
    /// Event raised when settings are loaded.
    /// </summary>
    public event EventHandler? SettingsLoaded;

    public SettingsManager()
    {
        _settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Gnomicon");
        _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
        Settings = new AppSettings();
    }

    /// <summary>
    /// Loads settings from the configuration file.
    /// Creates default settings if the file doesn't exist.
    /// </summary>
    public void Load()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (loadedSettings != null)
                    {
                        Settings = loadedSettings;
                    }
                }
                else
                {
                    // First run - create directory and save defaults
                    Directory.CreateDirectory(_settingsDirectory);
                    Save();
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with defaults
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        SettingsLoaded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves the current settings to the configuration file.
    /// </summary>
    public void Save()
    {
        lock (_lockObject)
        {
            try
            {
                Directory.CreateDirectory(_settingsDirectory);
                
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        SettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets the full path to the settings file.
    /// </summary>
    public string GetSettingsFilePath()
    {
        return _settingsFilePath;
    }
}

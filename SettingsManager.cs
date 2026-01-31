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

    public AppSettings Settings { get; private set; }

    public event EventHandler? SettingsSaved;
    public event EventHandler? SettingsLoaded;

    public SettingsManager()
    {
        _settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Gnomicon");
        _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
        Settings = new AppSettings();
    }

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
                    Directory.CreateDirectory(_settingsDirectory);
                    Save();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        SettingsLoaded?.Invoke(this, EventArgs.Empty);
    }

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

    public string GetSettingsFilePath()
    {
        return _settingsFilePath;
    }
}

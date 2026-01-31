using System.Text.Json.Serialization;

namespace Gnomicon;

/// <summary>
/// Application settings and configuration.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Whether the application is currently enabled to rearrange icons.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// The current rearrangement mode.
    /// </summary>
    public RearrangementMode Mode { get; set; } = RearrangementMode.Sneaky;

    /// <summary>
    /// If set, the application is paused until this time.
    /// </summary>
    public DateTime? PauseUntil { get; set; }

    /// <summary>
    /// Whether to show notifications after rearranging.
    /// </summary>
    public bool ShowNotifications { get; set; } = true;

    /// <summary>
    /// The interval in minutes between rearrangements.
    /// </summary>
    public int IntervalMinutes { get; set; } = 60;

    /// <summary>
    /// The saved original icon positions for restoration.
    /// </summary>
    public List<IconPosition> OriginalPositions { get; set; } = new();

    /// <summary>
    /// Whether original positions have been saved.
    /// </summary>
    [JsonIgnore]
    public bool HasOriginalPositions => OriginalPositions.Count > 0;

    /// <summary>
    /// Checks if the application is currently paused.
    /// </summary>
    [JsonIgnore]
    public bool IsPaused
    {
        get
        {
            if (PauseUntil == null)
                return false;
            
            if (DateTime.Now >= PauseUntil.Value)
            {
                // Auto-clear expired pause
                PauseUntil = null;
                return false;
            }
            
            return true;
        }
    }

    /// <summary>
    /// Gets the remaining pause time as a user-friendly string.
    /// </summary>
    [JsonIgnore]
    public string PauseRemainingText
    {
        get
        {
            if (PauseUntil == null)
                return "";
            
            var remaining = PauseUntil.Value - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
                return "";
            
            if (remaining.TotalMinutes < 1)
                return "< 1 min";
            
            return $"{remaining.TotalMinutes:F0} min";
        }
    }

    /// <summary>
    /// Pauses the application for the specified duration.
    /// </summary>
    public void PauseFor(TimeSpan duration)
    {
        PauseUntil = DateTime.Now.Add(duration);
    }

    /// <summary>
    /// Resumes the application by clearing the pause.
    /// </summary>
    public void Resume()
    {
        PauseUntil = null;
    }

    /// <summary>
    /// Saves the original icon positions.
    /// </summary>
    public void SaveOriginalPositions(List<IconPosition> positions)
    {
        OriginalPositions = positions.Select(p => p.Clone()).ToList();
    }

    /// <summary>
    /// Clears the saved original positions.
    /// </summary>
    public void ClearOriginalPositions()
    {
        OriginalPositions.Clear();
    }
}

using System.Text.JsonSerialization;

namespace Gnomicon;

/// <summary>
/// Application settings class.
/// </summary>
public class AppSettings
{
    public bool IsEnabled { get; set; } = true;
    public RearrangementMode Mode { get; set; } = RearrangementMode.Sneaky;
    public DateTime? PauseUntil { get; set; }
    public bool ShowNotifications { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public List<IconPosition> OriginalPositions { get; set; } = new();

    [JsonIgnore]
    public bool HasOriginalPositions => OriginalPositions.Count > 0;

    [JsonIgnore]
    public bool IsPaused
    {
        get
        {
            if (PauseUntil == null)
                return false;
            
            if (DateTime.Now >= PauseUntil.Value)
            {
                PauseUntil = null;
                return false;
            }
            
            return true;
        }
    }

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

    public void PauseFor(TimeSpan duration)
    {
        PauseUntil = DateTime.Now.Add(duration);
    }

    public void Resume()
    {
        PauseUntil = null;
    }

    public void SaveOriginalPositions(List<IconPosition> positions)
    {
        OriginalPositions = positions.Select(p => p.Clone()).ToList();
    }

    public void ClearOriginalPositions()
    {
        OriginalPositions.Clear();
    }
}

namespace Gnomicon;

/// <summary>
/// Defines the different modes for rearranging desktop icons.
/// </summary>
public enum RearrangementMode
{
    /// <summary>
    /// Completely randomizes all icon positions within the visible screen area.
    /// </summary>
    FullChaos,

    /// <summary>
    /// Subtly swaps only a few icon positions (2-4 icons).
    /// </summary>
    Sneaky,

    /// <summary>
    /// Rotates icons in a slow circular pattern around the screen center.
    /// </summary>
    Orbit
}

/// <summary>
/// Extension methods for RearrangementMode enum.
/// </summary>
public static class RearrangementModeExtensions
{
    /// <summary>
    /// Gets a user-friendly display name for the mode.
    /// </summary>
    public static string GetDisplayName(this RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => "Full Chaos",
            RearrangementMode.Sneaky => "Sneaky",
            RearrangementMode.Orbit => "Orbit",
            _ => mode.ToString()
        };
    }

    /// <summary>
    /// Gets a description of what the mode does.
    /// </summary>
    public static string GetDescription(this RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => "Randomizes all icon positions",
            RearrangementMode.Sneaky => "Swaps only 2-4 icon positions",
            RearrangementMode.Orbit => "Rotates icons in a circular pattern",
            _ => "Unknown mode"
        };
    }
}

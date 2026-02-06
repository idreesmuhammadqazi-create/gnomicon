namespace Gnomicon;

/// <summary>
/// Defines the different modes for rearranging desktop icons.
/// </summary>
public enum RearrangementMode
{
    FullChaos,
    Sneaky,
    Orbit,
    Word
}

public static class RearrangementModeExtensions
{
    public static string GetDisplayName(this RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => "Full Chaos",
            RearrangementMode.Sneaky => "Sneaky",
            RearrangementMode.Orbit => "Orbit",
            RearrangementMode.Word => "Word Mode",
            _ => mode.ToString()
        };
    }

    public static string GetDescription(this RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => "Randomizes all icon positions",
            RearrangementMode.Sneaky => "Swaps only 2-4 icon positions",
            RearrangementMode.Orbit => "Rotates icons in a circular pattern",
            RearrangementMode.Word => "Arranges icons to spell out fun words",
            _ => "Unknown mode"
        };
    }
}

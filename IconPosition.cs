namespace Gnomicon;

/// <summary>
/// Represents the position of a desktop icon.
/// </summary>
public class IconPosition
{
    /// <summary>
    /// The index of the icon in the desktop ListView.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The X coordinate of the icon position.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The Y coordinate of the icon position.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Optional name of the icon for debugging purposes.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creates a copy of this IconPosition.
    /// </summary>
    public IconPosition Clone()
    {
        return new IconPosition
        {
            Index = Index,
            X = X,
            Y = Y,
            Name = Name
        };
    }

    /// <summary>
    /// Returns a string representation of the icon position.
    /// </summary>
    public override string ToString()
    {
        return $"Icon[{Index}] at ({X}, {Y})" + (Name != null ? $" - {Name}" : "");
    }
}

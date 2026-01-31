namespace Gnomicon;

/// <summary>
/// Represents the position of a desktop icon.
/// </summary>
public class IconPosition
{
    public int Index { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string? Name { get; set; }

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

    public override string ToString()
    {
        return $"Icon[{Index}] at ({X}, {Y})" + (Name != null ? $" - {Name}" : "");
    }
}

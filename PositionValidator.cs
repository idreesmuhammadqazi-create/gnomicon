namespace Gnomicon;

/// <summary>
/// Validates and ensures icon positions stay within safe bounds.
/// </summary>
public class PositionValidator
{
    private readonly int _iconWidth;
    private readonly int _iconHeight;
    private readonly int _margin;

    public PositionValidator(int iconWidth = 48, int iconHeight = 48, int margin = 10)
    {
        _iconWidth = iconWidth;
        _iconHeight = iconHeight;
        _margin = margin;
    }

    /// <summary>
    /// Gets the safe working area for placing icons (excludes taskbar).
    /// </summary>
    public Rectangle GetSafeWorkingArea()
    {
        var workingArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        
        // Add margins to prevent icons from being too close to edges
        return new Rectangle(
            workingArea.Left + _margin,
            workingArea.Top + _margin,
            workingArea.Width - (_margin * 2),
            workingArea.Height - (_margin * 2)
        );
    }

    /// <summary>
    /// Validates and clamps a position to ensure it's within safe bounds.
    /// </summary>
    public Point ValidatePosition(int x, int y)
    {
        var safeArea = GetSafeWorkingArea();
        
        // Ensure the icon is fully visible (accounting for icon size)
        int safeX = Math.Max(safeArea.Left, Math.Min(x, safeArea.Right - _iconWidth));
        int safeY = Math.Max(safeArea.Top, Math.Min(y, safeArea.Bottom - _iconHeight));
        
        return new Point(safeX, safeY);
    }

    /// <summary>
    /// Validates and clamps a position to ensure it's within safe bounds.
    /// </summary>
    public Point ValidatePosition(Point point)
    {
        return ValidatePosition(point.X, point.Y);
    }

    /// <summary>
    /// Generates a random position within the safe working area.
    /// </summary>
    public Point GetRandomPosition(Random random)
    {
        var safeArea = GetSafeWorkingArea();
        
        int x = random.Next(safeArea.Left, safeArea.Right - _iconWidth);
        int y = random.Next(safeArea.Top, safeArea.Bottom - _iconHeight);
        
        return new Point(x, y);
    }

    /// <summary>
    /// Checks if a position is within the safe working area.
    /// </summary>
    public bool IsValidPosition(int x, int y)
    {
        var safeArea = GetSafeWorkingArea();
        
        return x >= safeArea.Left &&
               x <= safeArea.Right - _iconWidth &&
               y >= safeArea.Top &&
               y <= safeArea.Bottom - _iconHeight;
    }

    /// <summary>
    /// Ensures a list of positions don't overlap with each other (minimum spacing).
    /// </summary>
    public List<IconPosition> PreventOverlaps(List<IconPosition> positions, int minSpacing = 60)
    {
        var result = new List<IconPosition>();
        var random = new Random();
        var safeArea = GetSafeWorkingArea();

        foreach (var pos in positions)
        {
            var newPos = pos.Clone();
            int attempts = 0;
            const int maxAttempts = 50;

            // Try to find a non-overlapping position
            while (attempts < maxAttempts && HasOverlap(newPos, result, minSpacing))
            {
                newPos.X = random.Next(safeArea.Left, safeArea.Right - _iconWidth);
                newPos.Y = random.Next(safeArea.Top, safeArea.Bottom - _iconHeight);
                attempts++;
            }

            result.Add(newPos);
        }

        return result;
    }

    /// <summary>
    /// Checks if a position would overlap with any existing positions.
    /// </summary>
    private bool HasOverlap(IconPosition newPos, List<IconPosition> existing, int minSpacing)
    {
        foreach (var pos in existing)
        {
            int dx = Math.Abs(newPos.X - pos.X);
            int dy = Math.Abs(newPos.Y - pos.Y);
            
            if (dx < minSpacing && dy < minSpacing)
                return true;
        }
        
        return false;
    }
}

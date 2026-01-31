namespace Gnomicon;

/// <summary>
/// Validates and ensures icon positions stay within safe bounds.
/// Note: This handles all the positioning logic to keep icons visible on screen
/// </summary>
public class PositionValidator
{
    private readonly int _iconWidth;
    private readonly int _iconHeight;
    private readonly int _margin;

    // Constructor - pretty straightforward setup here
    public PositionValidator(int iconWidth = 48, int iconHeight = 48, int margin = 10)
    {
        _iconWidth = iconWidth;
        _iconHeight = iconHeight;
        _margin = margin;
    }

    // Gets the working area minus taskbar and adds our margin
    public Rectangle GetSafeWorkingArea()
    {
        // Fallback to 1920x1080 if we can't get the actual screen size for some reason
        var workingArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        
        // Apply margins on all sides
        return new Rectangle(
            workingArea.Left + _margin,
            workingArea.Top + _margin,
            workingArea.Width - (_margin * 2),
            workingArea.Height - (_margin * 2)
        );
    }

    // Main validation method - makes sure position is within bounds
    public Point ValidatePosition(int x, int y)
    {
        var safeArea = GetSafeWorkingArea();
        
        // Clamp X coordinate to safe range
        int safeX = Math.Max(safeArea.Left, Math.Min(x, safeArea.Right - _iconWidth));
        // Clamp Y coordinate to safe range
        int safeY = Math.Max(safeArea.Top, Math.Min(y, safeArea.Bottom - _iconHeight));
        
        return new Point(safeX, safeY);
    }

    // Overload for Point parameter - just calls the main method
    public Point ValidatePosition(Point point)
    {
        return ValidatePosition(point.X, point.Y);
    }

    // Generate a random position within safe area
    public Point GetRandomPosition(Random random)
    {
        var safeArea = GetSafeWorkingArea();
        
        int x = random.Next(safeArea.Left, safeArea.Right - _iconWidth);
        int y = random.Next(safeArea.Top, safeArea.Bottom - _iconHeight);
        
        return new Point(x, y);
    }

    // Check if a position is valid without modifying it
    public bool IsValidPosition(int x, int y)
    {
        var safeArea = GetSafeWorkingArea();
        
        bool xValid = x >= safeArea.Left && x <= safeArea.Right - _iconWidth;
        bool yValid = y >= safeArea.Top && y <= safeArea.Bottom - _iconHeight;
        
        return xValid && yValid;
    }

    // TODO: This could be optimized with a spatial hash or something if we have lots of icons
    // For now it's fine though - just repositions overlapping icons
    public List<IconPosition> PreventOverlaps(List<IconPosition> positions, int minSpacing = 60)
    {
        var result = new List<IconPosition>();
        var random = new Random();
        var safeArea = GetSafeWorkingArea();

        // Go through each position and check for overlaps
        foreach (var pos in positions)
        {
            var newPos = pos.Clone();
            int attempts = 0;
            const int maxAttempts = 50;  // Give up after 50 tries to avoid infinite loops

            // Keep trying random positions until we find one that doesn't overlap
            while (attempts < maxAttempts && HasOverlap(newPos, result, minSpacing))
            {
                // Generate new random position
                newPos.X = random.Next(safeArea.Left, safeArea.Right - _iconWidth);
                newPos.Y = random.Next(safeArea.Top, safeArea.Bottom - _iconHeight);
                attempts++;
            }
            
            // Add it even if it still overlaps after max attempts - better than losing icons

            result.Add(newPos);
        }

        return result;
    }

    // Helper method to check if position overlaps with existing icons
    private bool HasOverlap(IconPosition newPos, List<IconPosition> existing, int minSpacing)
    {
        // Check distance to each existing icon
        foreach (var pos in existing)
        {
            int dx = Math.Abs(newPos.X - pos.X);
            int dy = Math.Abs(newPos.Y - pos.Y);
            
            // If both dimensions are closer than minSpacing, we have overlap
            if (dx < minSpacing && dy < minSpacing)
            {
                return true;
            }
        }
        
        return false;
    }
}
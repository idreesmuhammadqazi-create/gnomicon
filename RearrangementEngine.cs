namespace Gnomicon;

/// <summary>
/// Implements different icon rearrangement modes.
/// </summary>
public class RearrangementEngine
{
    private readonly PositionValidator _validator;
    private readonly Random _random;

    public RearrangementEngine(PositionValidator validator)
    {
        _validator = validator;
        _random = new Random();
    }

    /// <summary>
    /// Rearranges icons according to the specified mode.
    /// </summary>
    public List<IconPosition> Rearrange(List<IconPosition> currentPositions, RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => FullChaos(currentPositions),
            RearrangementMode.Sneaky => Sneaky(currentPositions),
            RearrangementMode.Orbit => Orbit(currentPositions),
            _ => currentPositions
        };
    }

    /// <summary>
    /// Full Chaos mode: Randomizes all icon positions within the visible screen area.
    /// </summary>
    private List<IconPosition> FullChaos(List<IconPosition> positions)
    {
        var newPositions = new List<IconPosition>();
        var safeArea = _validator.GetSafeWorkingArea();

        foreach (var pos in positions)
        {
            var newPos = pos.Clone();
            newPos.X = _random.Next(safeArea.Left, safeArea.Right - 48);
            newPos.Y = _random.Next(safeArea.Top, safeArea.Bottom - 48);
            newPositions.Add(newPos);
        }

        // Prevent overlaps
        return _validator.PreventOverlaps(newPositions, 60);
    }

    /// <summary>
    /// Sneaky mode: Randomly swaps only 2-4 icon positions.
    /// </summary>
    private List<IconPosition> Sneaky(List<IconPosition> positions)
    {
        if (positions.Count < 2)
            return positions;

        var newPositions = positions.Select(p => p.Clone()).ToList();
        
        // Determine how many swaps to make (2-4 icons affected)
        int swapCount = Math.Min(_random.Next(2, 5), positions.Count / 2);
        
        // Create a list of indices to swap
        var indices = Enumerable.Range(0, positions.Count).ToList();
        
        for (int i = 0; i < swapCount; i++)
        {
            if (indices.Count < 2)
                break;

            // Pick two random indices
            int idx1 = _random.Next(indices.Count);
            int index1 = indices[idx1];
            indices.RemoveAt(idx1);

            int idx2 = _random.Next(indices.Count);
            int index2 = indices[idx2];
            indices.RemoveAt(idx2);

            // Swap their positions
            int tempX = newPositions[index1].X;
            int tempY = newPositions[index1].Y;
            
            newPositions[index1].X = newPositions[index2].X;
            newPositions[index1].Y = newPositions[index2].Y;
            
            newPositions[index2].X = tempX;
            newPositions[index2].Y = tempY;
        }

        return newPositions;
    }

    /// <summary>
    /// Orbit mode: Rotates icons in a slow circular pattern around the screen center.
    /// </summary>
    private List<IconPosition> Orbit(List<IconPosition> positions)
    {
        if (positions.Count == 0)
            return positions;

        var safeArea = _validator.GetSafeWorkingArea();
        int centerX = safeArea.Left + safeArea.Width / 2;
        int centerY = safeArea.Top + safeArea.Height / 2;

        // Calculate the orbit radius based on screen size
        int maxRadius = Math.Min(safeArea.Width, safeArea.Height) / 3;
        
        var newPositions = new List<IconPosition>();
        
        // Distribute icons evenly around the circle
        double angleStep = 2 * Math.PI / positions.Count;
        double baseAngle = _random.NextDouble() * 2 * Math.PI; // Random starting rotation

        for (int i = 0; i < positions.Count; i++)
        {
            var pos = positions[i].Clone();
            
            // Calculate position on the circle
            double angle = baseAngle + (i * angleStep);
            
            // Vary the radius slightly for each icon to create a more natural look
            int radius = maxRadius - (i % 3) * 30; // 3 concentric rings
            
            int newX = centerX + (int)(radius * Math.Cos(angle)) - 24; // Center the icon
            int newY = centerY + (int)(radius * Math.Sin(angle)) - 24;
            
            // Validate and clamp to safe area
            var validated = _validator.ValidatePosition(newX, newY);
            pos.X = validated.X;
            pos.Y = validated.Y;
            
            newPositions.Add(pos);
        }

        return newPositions;
    }

    /// <summary>
    /// Gets a description of what the next rearrangement will do.
    /// </summary>
    public string GetRearrangementDescription(RearrangementMode mode, int iconCount)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => $"Will randomly reposition all {iconCount} icons",
            RearrangementMode.Sneaky => $"Will subtly swap 2-{Math.Min(4, iconCount)} icon positions",
            RearrangementMode.Orbit => $"Will arrange {iconCount} icons in a circular pattern",
            _ => "Unknown mode"
        };
    }
}

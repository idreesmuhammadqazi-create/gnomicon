namespace Gnomicon;

/// <summary>
/// Implements different icon rearrangement modes.
/// </summary>
public class RearrangementEngine
{
    private readonly PositionValidator _validator;
    private readonly Random _random;
    private WordPatternBuilder? _wordPatternBuilder;

    public RearrangementEngine(PositionValidator validator)
    {
        _validator = validator;
        _random = new Random();
    }

    private WordPatternBuilder WordPatternBuilder
    {
        get
        {
            _wordPatternBuilder ??= new WordPatternBuilder(_validator);
            return _wordPatternBuilder;
        }
    }

    public List<IconPosition> Rearrange(List<IconPosition> currentPositions, RearrangementMode mode)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => FullChaos(currentPositions),
            RearrangementMode.Sneaky => Sneaky(currentPositions),
            RearrangementMode.Orbit => Orbit(currentPositions),
            RearrangementMode.Word => WordRearrangement(currentPositions),
            _ => currentPositions
        };
    }

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

        return _validator.PreventOverlaps(newPositions, 60);
    }

    private List<IconPosition> Sneaky(List<IconPosition> positions)
    {
        if (positions.Count < 2)
            return positions;

        var newPositions = positions.Select(p => p.Clone()).ToList();
        
        int swapCount = Math.Min(_random.Next(2, 5), positions.Count / 2);
        
        var indices = Enumerable.Range(0, positions.Count).ToList();
        
        for (int i = 0; i < swapCount; i++)
        {
            if (indices.Count < 2)
                break;

            int idx1 = _random.Next(indices.Count);
            int index1 = indices[idx1];
            indices.RemoveAt(idx1);

            int idx2 = _random.Next(indices.Count);
            int index2 = indices[idx2];
            indices.RemoveAt(idx2);

            int tempX = newPositions[index1].X;
            int tempY = newPositions[index1].Y;
            
            newPositions[index1].X = newPositions[index2].X;
            newPositions[index1].Y = newPositions[index2].Y;
            
            newPositions[index2].X = tempX;
            newPositions[index2].Y = tempY;
        }

        return newPositions;
    }

    private List<IconPosition> Orbit(List<IconPosition> positions)
    {
        if (positions.Count == 0)
            return positions;

        var safeArea = _validator.GetSafeWorkingArea();
        int centerX = safeArea.Left + safeArea.Width / 2;
        int centerY = safeArea.Top + safeArea.Height / 2;

        int maxRadius = Math.Min(safeArea.Width, safeArea.Height) / 3;
        
        var newPositions = new List<IconPosition>();
        
        double angleStep = 2 * Math.PI / positions.Count;
        double baseAngle = _random.NextDouble() * 2 * Math.PI;

        for (int i = 0; i < positions.Count; i++)
        {
            var pos = positions[i].Clone();
            
            double angle = baseAngle + (i * angleStep);
            
            int radius = maxRadius - (i % 3) * 30;
            
            int newX = centerX + (int)(radius * Math.Cos(angle)) - 24;
            int newY = centerY + (int)(radius * Math.Sin(angle)) - 24;
            
            var validated = _validator.ValidatePosition(newX, newY);
            pos.X = validated.X;
            pos.Y = validated.Y;
            
            newPositions.Add(pos);
        }

        return newPositions;
    }

    private List<IconPosition> WordRearrangement(List<IconPosition> positions)
    {
        if (positions.Count < 3)
            return FullChaos(positions);

        // Select a random word
        var word = WordPatternBuilder.SelectRandomWord();
        
        // Check if we have enough icons
        var iconsNeeded = WordPatternBuilder.CalculateTotalIconsNeeded(word);
        
        if (positions.Count < iconsNeeded)
        {
            // Try to find a fitting word
            var fittingWord = WordPatternBuilder.FindFittingWord(positions.Count);
            if (fittingWord != null)
            {
                word = fittingWord;
                iconsNeeded = WordPatternBuilder.CalculateTotalIconsNeeded(word);
            }
        }

        // Build the word pattern
        var (wordPositions, extraPositions) = WordPatternBuilder.BuildWordPositions(word, positions);
        
        // Scatter extra icons randomly
        var scatteredExtras = WordPatternBuilder.ScatterExtraIcons(extraPositions);
        
        // Combine word positions and scattered extras
        var result = new List<IconPosition>(wordPositions);
        result.AddRange(scatteredExtras);
        
        return result;
    }

    public string GetRearrangementDescription(RearrangementMode mode, int iconCount)
    {
        return mode switch
        {
            RearrangementMode.FullChaos => $"Will randomly reposition all {iconCount} icons",
            RearrangementMode.Sneaky => $"Will subtly swap 2-{Math.Min(4, iconCount)} icon positions",
            RearrangementMode.Orbit => $"Will arrange {iconCount} icons in a circular pattern",
            RearrangementMode.Word => $"Will arrange icons to spell out a fun word",
            _ => "Unknown mode"
        };
    }
}

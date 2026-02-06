namespace Gnomicon;

/// <summary>
/// Calculates icon positions to form words using letter patterns.
/// </summary>
public class WordPatternBuilder
{
    private readonly PositionValidator _validator;
    private readonly Random _random;

    public WordPatternBuilder(PositionValidator validator)
    {
        _validator = validator;
        _random = new Random();
    }

    /// <summary>
    /// Builds word positions using available icons.
    /// Returns tuple of: (word positions, extra icon positions)
    /// </summary>
    public (List<IconPosition> wordPositions, List<IconPosition> extraPositions) 
        BuildWordPositions(string word, List<IconPosition> allPositions)
    {
        var wordUpper = word.ToUpperInvariant();
        var totalIconsNeeded = CalculateTotalIconsNeeded(wordUpper);
        
        if (allPositions.Count < totalIconsNeeded)
        {
            // Try to find a shorter word that fits
            var shorterWord = FindFittingWord(allPositions.Count);
            if (shorterWord != null)
            {
                wordUpper = shorterWord;
                totalIconsNeeded = CalculateTotalIconsNeeded(wordUpper);
            }
        }

        var wordPositions = new List<IconPosition>();
        var extraPositions = new List<IconPosition>();

        // Calculate starting position to center the word on screen
        var safeArea = _validator.GetSafeWorkingArea();
        int wordWidth = CalculateWordWidth(wordUpper);
        int startX = Math.Max(safeArea.Left, (safeArea.Width - wordWidth) / 2);
        int startY = Math.Max(safeArea.Top, (safeArea.Height - GridConfig.RowHeight) / 2);

        int currentX = startX;
        int iconIndex = 0;

        foreach (char letter in wordUpper)
        {
            if (!WordModeConstants.LetterPatterns.TryGetValue(letter, out var pattern))
            {
                currentX += GridConfig.LetterSpacing;
                continue;
            }

            foreach (var (row, col) in pattern)
            {
                if (iconIndex >= allPositions.Count)
                    break;

                var pos = allPositions[iconIndex].Clone();
                pos.X = currentX + (col * GridConfig.IconSpacing);
                pos.Y = startY + (row * GridConfig.IconSpacing);
                
                var validated = _validator.ValidatePosition(pos.X, pos.Y);
                pos.X = validated.X;
                pos.Y = validated.Y;
                
                wordPositions.Add(pos);
                iconIndex++;
            }

            currentX += GridConfig.LetterSpacing + (pattern.Max(p => p.Col) * GridConfig.IconSpacing);
        }

        // Collect remaining icons as extras
        for (int i = iconIndex; i < allPositions.Count; i++)
        {
            extraPositions.Add(allPositions[i].Clone());
        }

        return (wordPositions, extraPositions);
    }

    /// <summary>
    /// Randomly selects a fun word from the word list.
    /// </summary>
    public string SelectRandomWord()
    {
        return WordModeConstants.FunWords[_random.Next(WordModeConstants.FunWords.Length)];
    }

    /// <summary>
    /// Calculates the total number of icons needed to spell a word.
    /// </summary>
    public int CalculateTotalIconsNeeded(string word)
    {
        var total = 0;
        foreach (char letter in word.ToUpperInvariant())
        {
            if (WordModeConstants.IconCounts.TryGetValue(letter, out var count))
            {
                total += count;
            }
        }
        return total;
    }

    /// <summary>
    /// Finds a word from the list that fits within the available icon count.
    /// Prefers longer words when possible.
    /// </summary>
    public string? FindFittingWord(int availableIcons)
    {
        var fittingWords = WordModeConstants.FunWords
            .Where(w => CalculateTotalIconsNeeded(w) <= availableIcons)
            .OrderByDescending(w => w.Length)
            .ToList();

        return fittingWords.FirstOrDefault();
    }

    /// <summary>
    /// Calculates the width of a word in pixels based on letter patterns.
    /// </summary>
    private int CalculateWordWidth(string word)
    {
        var width = 0;
        foreach (char letter in word.ToUpperInvariant())
        {
            if (WordModeConstants.LetterPatterns.TryGetValue(letter, out var pattern))
            {
                var letterWidth = (pattern.Max(p => p.Col) + 1) * GridConfig.IconSpacing;
                width += letterWidth + GridConfig.LetterSpacing;
            }
        }
        return Math.Max(0, width - GridConfig.LetterSpacing); // Remove last letter spacing
    }

    /// <summary>
    /// Scatters extra icons randomly within the safe working area.
    /// </summary>
    public List<IconPosition> ScatterExtraIcons(List<IconPosition> extraPositions)
    {
        var safeArea = _validator.GetSafeWorkingArea();
        var scattered = new List<IconPosition>();

        foreach (var pos in extraPositions)
        {
            var newPos = pos.Clone();
            newPos.X = _random.Next(safeArea.Left, safeArea.Right - GridConfig.IconSpacing);
            newPos.Y = _random.Next(safeArea.Top, safeArea.Bottom - GridConfig.IconSpacing);
            
            var validated = _validator.ValidatePosition(newPos.X, newPos.Y);
            newPos.X = validated.X;
            newPos.Y = validated.Y;
            
            scattered.Add(newPos);
        }

        return scattered;
    }
}

namespace Gnomicon;

/// <summary>
/// Contains predefined word lists and letter patterns for Word Mode rearrangement.
/// </summary>
public static class WordModeConstants
{
    /// <summary>
    /// Fun words that can be spelled with desktop icons.
    /// </summary>
    public static readonly string[] FunWords = new[]
    {
        "LOL", "LMAO", "WOW", "OK", "HI", "BYE", "YES", "NO",
        "HAHA", "OMG", "WIN", "FAIL", "NICE", "COOL", "CUTE",
        "EPIC", "BOSS", "GGWP", "FTW", "MVP", "LOVE", "HATE",
        "GOOD", "BAD", "HOT", "SAD", "MAD", "FUN", "GAME",
        "GEEK", "NERD", "PRO", "TOP", "BIG", "NEW", "DAY",
        "SUN", "SKY", "RED", "BLUE", "ALL", "OUT", "RUN"
    };

    /// <summary>
    /// Icon grid patterns for each letter using (row, col) coordinates.
    /// Each letter is designed to be recognizable with minimal icons.
    /// </summary>
    public static readonly Dictionary<char, List<(int Row, int Col)>> LetterPatterns = new()
    {
        // Simple letters (3 icons)
        ['I'] = new() { (0, 1), (1, 1), (2, 1) },
        ['L'] = new() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) },
        ['T'] = new() { (0, 0), (0, 1), (0, 2), (1, 1), (2, 1) },
        
        // Medium letters (4-5 icons)
        ['A'] = new() { (0, 1), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2) },
        ['C'] = new() { (0, 1), (0, 2), (1, 0), (2, 0), (2, 1), (2, 2) },
        ['E'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (2, 0), (2, 1), (2, 2) },
        ['F'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (2, 0), (2, 1) },
        ['H'] = new() { (0, 0), (1, 0), (2, 0), (0, 2), (1, 2), (2, 2) },
        ['J'] = new() { (0, 1), (0, 2), (1, 2), (2, 0), (2, 1), (2, 2) },
        ['K'] = new() { (0, 0), (1, 0), (2, 0), (1, 1), (1, 2) },
        ['N'] = new() { (0, 0), (1, 0), (1, 1), (1, 2), (2, 2), (3, 2) },
        ['O'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 3), (3, 1), (3, 2) },
        ['P'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1) },
        ['S'] = new() { (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1) },
        ['U'] = new() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2), (3, 2) },
        ['V'] = new() { (0, 0), (1, 0), (2, 1), (3, 2), (4, 2) },
        ['W'] = new() { (0, 0), (1, 0), (2, 1), (3, 2), (4, 2), (4, 3) },
        ['X'] = new() { (0, 0), (0, 3), (1, 1), (1, 2), (2, 1), (2, 2), (3, 0), (3, 3) },
        ['Y'] = new() { (0, 0), (0, 3), (1, 1), (1, 2), (2, 1), (3, 1), (4, 1) },
        ['Z'] = new() { (0, 0), (0, 1), (0, 2), (1, 2), (2, 0), (2, 1), (2, 2) },
        
        // Letters with descenders
        ['G'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 2), (2, 3), (3, 2) },
        ['Q'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 3), (3, 1), (3, 2), (3, 3) },
        ['R'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (3, 2) },
        
        // Complex letters (more icons for clarity)
        ['B'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2), (3, 0), (3, 1), (3, 2), (4, 0), (4, 1) },
        ['D'] = new() { (0, 0), (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 3), (3, 0), (3, 1), (3, 2), (4, 0), (4, 1) },
        ['M'] = new() { (0, 0), (0, 5), (1, 0), (1, 2), (1, 3), (1, 5), (2, 0), (2, 1), (2, 4), (2, 5), (3, 0), (3, 5), (4, 0), (4, 5) },
    };

    /// <summary>
    /// Gets the icon count needed for each letter.
    /// </summary>
    public static Dictionary<char, int> IconCounts { get; } = new()
    {
        ['A'] = 6, ['B'] = 13, ['C'] = 6, ['D'] = 11, ['E'] = 7,
        ['F'] = 6, ['G'] = 8, ['H'] = 6, ['I'] = 3, ['J'] = 6,
        ['K'] = 5, ['L'] = 5, ['M'] = 14, ['N'] = 6, ['O'] = 8,
        ['P'] = 7, ['Q'] = 9, ['R'] = 8, ['S'] = 6, ['T'] = 5,
        ['U'] = 6, ['V'] = 5, ['W'] = 6, ['X'] = 8, ['Y'] = 7,
        ['Z'] = 7
    };

    /// <summary>
    /// Configuration for icon grid layout.
    /// </summary>
    public static class GridConfig
    {
        public static int IconSpacing { get; set; } = 60;
        public static int LetterSpacing { get; set; } = 40;
        public static int RowHeight { get; set; } = 60;
        public static int StartX { get; set; } = 100;
        public static int StartY { get; set; } = 200;
    }
}

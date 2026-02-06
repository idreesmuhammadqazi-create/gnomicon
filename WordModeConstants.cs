namespace Gnomicon;

/// <summary>
/// Contains predefined word lists and letter patterns for Word Mode rearrangement.
/// </summary>
public static class WordModeConstants
{
    /// <summary>
    /// 3-letter fun words that are easy to recognize.
    /// </summary>
    public static readonly string[] FunWords = new[]
    {
        "LOL", "WOW", "OK", "HI", "BYE", "YES", "NO",
        "WIN","BAD",  "SAD", "MAD", "FUN"
    };

    /// <summary>
    /// Simple, bold letter patterns that are easy to recognize from a distance.
    /// Each letter uses 3x3 or 3x4 grids for optimal clarity.
    /// </summary>
    public static readonly Dictionary<char, List<(int Row, int Col)>> LetterPatterns = new()
    {
        // 3x3 grid letters (simple and bold)
        ['I'] = new() { (0, 1), (1, 1), (2, 1) }, // Vertical line
        ['L'] = new() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) }, // Vertical line with horizontal base
        ['T'] = new() { (0, 0), (0, 1), (0, 2), (1, 1), (2, 1) }, // Horizontal top with vertical stem
        ['O'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 3), (3, 0), (3, 3), (4, 1), (4, 2) }, // Hollow square (O shape)
        ['X'] = new() { (0, 0), (0, 3), (1, 1), (1, 2), (2, 0), (2, 3), (3, 1), (3, 2), (4, 0), (4, 3) }, // Cross pattern
        
        // 3x4 grid letters (medium complexity)
        ['A'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 1), (2, 2), (2, 3), (3, 0), (3, 3), (4, 1), (4, 2) }, // Triangular shape
        ['E'] = new() { (0, 0), (0, 1), (0, 2), (0, 3), (1, 0), (2, 0), (2, 1), (2, 2), (3, 0), (4, 0), (4, 1), (4, 2), (4, 3) }, // Three horizontal lines
        ['F'] = new() { (0, 0), (0, 1), (0, 2), (0, 3), (1, 0), (2, 0), (2, 1), (2, 2), (3, 0) }, // Two horizontal lines
        ['H'] = new() { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (0, 3), (1, 3), (2, 3), (3, 3), (4, 3), (2, 1), (2, 2) }, // Two vertical lines with crossbar
        ['N'] = new() { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (0, 3), (1, 3), (2, 3), (3, 3), (4, 3), (1, 1), (2, 2), (3, 1) }, // Diagonal bridge
        ['S'] = new() { (0, 1), (0, 2), (1, 0), (1, 3), (2, 0), (2, 1), (2, 2), (2, 3), (3, 0), (3, 3), (4, 1), (4, 2) }, // S curve shape
    };

    /// <summary>
    /// Gets the icon count needed for each letter.
    /// </summary>
    public static Dictionary<char, int> IconCounts { get; } = new()
    {
        ['A'] = 12, ['B'] = 10, ['C'] = 8, ['D'] = 10, ['E'] = 14,
        ['F'] = 10, ['G'] = 10, ['H'] = 12, ['I'] = 3, ['J'] = 8,
        ['K'] = 10, ['L'] = 5, ['M'] = 20, ['N'] = 14, ['O'] = 10,
        ['P'] = 10, ['Q'] = 10, ['R'] = 10, ['S'] = 12, ['T'] = 5,
        ['U'] = 10, ['V'] = 8, ['W'] = 14, ['X'] = 10, ['Y'] = 8,
        ['Z'] = 10
    };

    /// <summary>
    /// Configuration for icon grid layout. Using larger spacing for better visibility.
    /// </summary>
    public static class GridConfig
    {
        public static int IconSpacing { get; set; } = 80; // Larger spacing for better visibility
        public static int LetterSpacing { get; set; } = 60; // More space between letters
        public static int RowHeight { get; set; } = 80; // Larger vertical spacing
        public static int StartX { get; set; } = 100;
        public static int StartY { get; set; } = 200;
    }
}

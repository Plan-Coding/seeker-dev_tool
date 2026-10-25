using System.Text.RegularExpressions;

namespace Seeker.DevToolbox.Core.Tools.Text;

public static class TextTransform
{
    private static readonly Regex WordRegex = new(
        @"[A-Z]+(?![a-z])|[A-Z][a-z]*|[a-z]+|\d+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static IReadOnlyList<string> SplitWords(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return WordRegex.Matches(input).Select(match => match.Value).ToList();
    }

    public static string ToCamelCase(string input)
    {
        var words = SplitWords(input);

        return words.Count == 0
            ? string.Empty
            : Lower(words[0]) + string.Concat(words.Skip(1).Select(Pascal));
    }

    public static string ToPascalCase(string input) =>
        string.Concat(SplitWords(input).Select(Pascal));

    public static string ToSnakeCase(string input) => JoinWords(input, "_", Lower);

    public static string ToKebabCase(string input) => JoinWords(input, "-", Lower);

    public static string ToConstantCase(string input) => JoinWords(input, "_", Upper);

    public static string ToTitleCase(string input) => JoinWords(input, " ", Pascal);

    public static string SortLines(string input, bool descending = false, bool ignoreCase = true)
    {
        var (lines, newLine) = SplitLines(input);
        var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        var sorted = descending
            ? lines.OrderByDescending(line => line, comparer)
            : lines.OrderBy(line => line, comparer);

        return string.Join(newLine, sorted);
    }

    public static string RemoveDuplicateLines(string input, bool ignoreCase = true, bool trim = false)
    {
        var (lines, newLine) = SplitLines(input);
        var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var seen = new HashSet<string>(comparer);
        var result = new List<string>(lines.Length);

        foreach (var line in lines)
        {
            var key = trim ? line.Trim() : line;

            if (seen.Add(key))
            {
                result.Add(line);
            }
        }

        return string.Join(newLine, result);
    }

    public static string RemoveEmptyLines(string input)
    {
        var (lines, newLine) = SplitLines(input);
        return string.Join(newLine, lines.Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    public static string TrimLines(string input)
    {
        var (lines, newLine) = SplitLines(input);
        return string.Join(newLine, lines.Select(line => line.Trim()));
    }

    public static string AddLineNumbers(string input, int start = 1, string separator = ". ")
    {
        var (lines, newLine) = SplitLines(input);

        if (lines.Length == 0)
        {
            return string.Empty;
        }

        start = Math.Max(0, start);
        var last = start + lines.Length - 1;
        var width = last.ToString().Length;

        return string.Join(
            newLine,
            lines.Select((line, index) =>
                (start + index).ToString().PadLeft(width) + separator + line));
    }

    private static string JoinWords(string input, string separator, Func<string, string> transform) =>
        string.Join(separator, SplitWords(input).Select(transform));

    private static string Lower(string word) =>
        word.Length == 0 ? word : char.ToLowerInvariant(word[0]) + word[1..].ToLowerInvariant();

    private static string Upper(string word) => word.ToUpperInvariant();

    private static string Pascal(string word) =>
        word.Length == 0 ? word : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();

    private static (string[] Lines, string NewLine) SplitLines(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var newLine = input.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        return (input.Replace("\r\n", "\n").Split('\n'), newLine);
    }
}

using System.Text.RegularExpressions;

namespace Seeker.DevToolbox.Core.Tools.Text;

public sealed record RegexGroupInfo(int Number, string Name, bool Success, string Value);

public sealed record RegexMatchInfo(int Index, int Length, string Value, IReadOnlyList<RegexGroupInfo> Groups);

public static class RegexTool
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);

    public static bool TryMatches(
        string pattern,
        string input,
        RegexOptions options,
        int maxMatches,
        out IReadOnlyList<RegexMatchInfo> matches,
        out string? error)
    {
        matches = [];

        try
        {
            var regex = new Regex(pattern, options, MatchTimeout);
            var result = new List<RegexMatchInfo>();

            foreach (Match match in regex.Matches(input))
            {
                if (result.Count >= maxMatches)
                {
                    break;
                }

                var groups = new List<RegexGroupInfo>(match.Groups.Count);

                for (var i = 0; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    groups.Add(new RegexGroupInfo(
                        Number: i,
                        Name: regex.GroupNameFromNumber(i),
                        Success: group.Success,
                        Value: group.Success ? group.Value : string.Empty));
                }

                result.Add(new RegexMatchInfo(match.Index, match.Length, match.Value, groups));
            }

            matches = result;
            error = null;
            return true;
        }
        catch (ArgumentException ex)
        {
            error = $"正则表达式无效:{ex.Message}";
            return false;
        }
        catch (RegexMatchTimeoutException)
        {
            error = "匹配超时,请简化表达式或减少输入长度";
            return false;
        }
    }

    public static bool TryReplace(
        string pattern,
        string input,
        string replacement,
        RegexOptions options,
        out string result,
        out string? error)
    {
        try
        {
            var regex = new Regex(pattern, options, MatchTimeout);
            result = regex.Replace(input, replacement);
            error = null;
            return true;
        }
        catch (ArgumentException ex)
        {
            result = string.Empty;
            error = $"正则表达式无效:{ex.Message}";
            return false;
        }
        catch (RegexMatchTimeoutException)
        {
            result = string.Empty;
            error = "替换超时,请简化表达式或减少输入长度";
            return false;
        }
    }
}

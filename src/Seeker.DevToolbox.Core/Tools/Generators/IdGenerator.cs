namespace Seeker.DevToolbox.Core.Tools.Generators;

public static class IdGenerator
{
    public const int MaxCount = 1000;

    public static IReadOnlyList<string> Generate(
        int count,
        bool version7 = false,
        bool uppercase = false,
        bool includeHyphens = true)
    {
        count = Math.Clamp(count, 1, MaxCount);

        var format = includeHyphens ? "D" : "N";
        var results = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var value = version7 ? Guid.CreateVersion7() : Guid.NewGuid();
            var text = value.ToString(format);
            results.Add(uppercase ? text.ToUpperInvariant() : text);
        }

        return results;
    }
}

using Seeker.DevToolbox.Core.Tools.Text;

namespace Seeker.DevToolbox.Tools.Text;

public sealed record DiffLineItem(DiffLineKind Kind, string Position, string Prefix, string Text)
{
    public static DiffLineItem From(DiffLine line) => new(
        line.Kind,
        line.Position?.ToString() ?? string.Empty,
        line.Kind switch
        {
            DiffLineKind.Added => "+",
            DiffLineKind.Removed => "-",
            DiffLineKind.Modified => "~",
            _ => " "
        },
        line.Text);
}

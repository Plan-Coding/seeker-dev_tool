using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace Seeker.DevToolbox.Core.Tools.Text;

public enum DiffLineKind
{
    Unchanged,
    Added,
    Removed,
    Modified
}

public sealed record DiffLine(DiffLineKind Kind, int? Position, string Text);

public sealed record TextDiffResult(
    bool HasDifferences,
    int AddedCount,
    int RemovedCount,
    int ModifiedCount,
    IReadOnlyList<DiffLine> Lines)
{
    public string UnifiedText => string.Join(
        Environment.NewLine,
        Lines.Select(line => line.Kind switch
        {
            DiffLineKind.Added => "+ " + line.Text,
            DiffLineKind.Removed => "- " + line.Text,
            DiffLineKind.Modified => "~ " + line.Text,
            _ => "  " + line.Text
        }));
}

public static class TextDiffService
{
    public static TextDiffResult Compare(
        string oldText,
        string newText,
        bool ignoreWhitespace = true)
    {
        var model = InlineDiffBuilder.Instance.BuildDiffModel(
            oldText ?? string.Empty,
            newText ?? string.Empty,
            ignoreWhitespace);

        var lines = new List<DiffLine>(model.Lines.Count);
        var added = 0;
        var removed = 0;
        var modified = 0;

        foreach (var piece in model.Lines)
        {
            switch (piece.Type)
            {
                case ChangeType.Imaginary:
                    continue;

                case ChangeType.Inserted:
                    added++;
                    lines.Add(new DiffLine(DiffLineKind.Added, piece.Position, piece.Text ?? string.Empty));
                    break;

                case ChangeType.Deleted:
                    removed++;
                    lines.Add(new DiffLine(DiffLineKind.Removed, piece.Position, piece.Text ?? string.Empty));
                    break;

                case ChangeType.Modified:
                    modified++;
                    lines.Add(new DiffLine(DiffLineKind.Modified, piece.Position, piece.Text ?? string.Empty));
                    break;

                default:
                    lines.Add(new DiffLine(DiffLineKind.Unchanged, piece.Position, piece.Text ?? string.Empty));
                    break;
            }
        }

        return new TextDiffResult(
            model.HasDifferences,
            added,
            removed,
            modified,
            lines);
    }
}

using Seeker.DevToolbox.Core.Tools.Text;

namespace Seeker.DevToolbox.Core.Tests;

public class TextDiffServiceTests
{
    [Fact]
    public void Compare_IdenticalTexts_NoDifferences()
    {
        var result = TextDiffService.Compare("alpha\nbeta", "alpha\nbeta");

        Assert.False(result.HasDifferences);
        Assert.All(result.Lines, line => Assert.Equal(DiffLineKind.Unchanged, line.Kind));
        Assert.Equal(0, result.AddedCount);
        Assert.Equal(0, result.RemovedCount);
    }

    [Fact]
    public void Compare_AddedLine_IsDetected()
    {
        var result = TextDiffService.Compare("alpha", "alpha\nbeta");

        Assert.True(result.HasDifferences);
        Assert.Equal(1, result.AddedCount);
        Assert.Contains(result.Lines, line => line.Kind == DiffLineKind.Added && line.Text == "beta");
        Assert.Contains("+ beta", result.UnifiedText);
    }

    [Fact]
    public void Compare_RemovedLine_IsDetected()
    {
        var result = TextDiffService.Compare("alpha\nbeta", "alpha");

        Assert.True(result.HasDifferences);
        Assert.Equal(1, result.RemovedCount);
        Assert.Contains(result.Lines, line => line.Kind == DiffLineKind.Removed && line.Text == "beta");
        Assert.Contains("- beta", result.UnifiedText);
    }

    [Fact]
    public void Compare_WordChange_ProducesDifference()
    {
        var result = TextDiffService.Compare("hello world", "hello brave world");

        Assert.True(result.HasDifferences);
        Assert.Contains(result.Lines, line => line.Kind is DiffLineKind.Modified or DiffLineKind.Added or DiffLineKind.Removed);
    }

    [Fact]
    public void Compare_IgnoreWhitespace_HidesTrailingWhitespaceChanges()
    {
        var strict = TextDiffService.Compare("a", "a ", ignoreWhitespace: false);
        var relaxed = TextDiffService.Compare("a", "a ", ignoreWhitespace: true);

        Assert.True(strict.HasDifferences);
        Assert.False(relaxed.HasDifferences);
    }
}

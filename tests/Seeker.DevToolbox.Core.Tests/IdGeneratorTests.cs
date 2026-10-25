using Seeker.DevToolbox.Core.Tools.Generators;

namespace Seeker.DevToolbox.Core.Tests;

public class IdGeneratorTests
{
    [Fact]
    public void Generate_UuidV4_HasVersionNibble()
    {
        var items = IdGenerator.Generate(10);

        Assert.Equal(10, items.Count);
        Assert.All(items, item => Assert.Equal('4', item[14]));
        Assert.All(items, item => Assert.Equal(36, item.Length));
    }

    [Fact]
    public void Generate_UuidV7_HasVersionNibble()
    {
        var items = IdGenerator.Generate(10, version7: true);

        Assert.All(items, item => Assert.Equal('7', item[14]));
    }

    [Fact]
    public void Generate_UppercaseWithoutHyphens()
    {
        var items = IdGenerator.Generate(3, uppercase: true, includeHyphens: false);

        Assert.All(items, item =>
        {
            Assert.Equal(32, item.Length);
            Assert.DoesNotContain('-', item);
            Assert.Equal(item.ToUpperInvariant(), item);
        });
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(5000, IdGenerator.MaxCount)]
    public void Generate_ClampsCount(int requested, int expected)
    {
        Assert.Equal(expected, IdGenerator.Generate(requested).Count);
    }

    [Fact]
    public void Generate_ReturnsUniqueValues()
    {
        var items = IdGenerator.Generate(1000);

        Assert.Equal(items.Count, items.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }
}
